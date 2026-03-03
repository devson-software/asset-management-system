using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Behaviors;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Common.Extensions;
using AssetPro.Api.Infrastructure.Auth;
using AssetPro.Api.Infrastructure.Database;
using AssetPro.Api.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// NLog
// ---------------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// ---------------------------------------------------------------------------
// Configuration sections
// ---------------------------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<SendGridSettings>(builder.Configuration.GetSection(SendGridSettings.SectionName));

// ---------------------------------------------------------------------------
// Database
// ---------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("AssetProDb")
    ?? throw new InvalidOperationException("Connection string 'AssetProDb' not configured.");

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

// EF Core — used ONLY for ASP.NET Identity tables
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------------------------------------------------------------------------
// ASP.NET Identity
// ---------------------------------------------------------------------------
builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

// ---------------------------------------------------------------------------
// Authentication & Authorization
// ---------------------------------------------------------------------------
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------------------
// MediatR + Pipeline Behaviors
// ---------------------------------------------------------------------------
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// ---------------------------------------------------------------------------
// FluentValidation
// ---------------------------------------------------------------------------
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ---------------------------------------------------------------------------
// Application services
// ---------------------------------------------------------------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, HttpUserContext>();
builder.Services.AddScoped<ITenantContext, HttpUserContext>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddTransient<IEmailService, SendGridEmailService>();

// ---------------------------------------------------------------------------
// Exception handling & CORS
// ---------------------------------------------------------------------------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ---------------------------------------------------------------------------
// OpenAPI / Scalar
// ---------------------------------------------------------------------------
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new()
        {
            Title = "AssetPro API",
            Version = "v1",
            Description = "HVAC Asset Management SaaS — Multi-tenant API"
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:9000"])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ---------------------------------------------------------------------------
// Build
// ---------------------------------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------------------------------
// Run DbUp migrations (Development only)
// ---------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    var migrationResult = DatabaseMigrator.Migrate(connectionString);
    if (!migrationResult.Successful)
    {
        app.Logger.LogError(migrationResult.Error, "Database migration failed.");
        throw migrationResult.Error;
    }
    app.Logger.LogInformation("Database migrations completed successfully. Scripts executed: {Count}", migrationResult.Scripts.Count());
}

// ---------------------------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------------------------
app.UseExceptionHandler();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("AssetPro API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// ---------------------------------------------------------------------------
// Health check
// ---------------------------------------------------------------------------
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }))
    .WithTags("System")
    .AllowAnonymous();

// ---------------------------------------------------------------------------
// Manual migration trigger (Admin only)
// ---------------------------------------------------------------------------
app.MapPost("/admin/migrate", async (IDbConnectionFactory db, ILogger<Program> logger) =>
{
    try
    {
        var result = DatabaseMigrator.Migrate(connectionString);
        if (!result.Successful)
        {
            logger.LogError(result.Error, "Manual database migration failed");
            return Results.Problem($"Migration failed: {result.Error.Message}", statusCode: 500);
        }
        
        logger.LogInformation("Manual database migration completed successfully");
        return Results.Ok(new { message = "Migration completed successfully", scripts = result.Scripts.Count() });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error during manual migration");
        return Results.Problem($"Unexpected error: {ex.Message}", statusCode: 500);
    }
})
.RequireAuthorization("Admin") // Only admins can trigger migrations
.WithTags("System");

// ---------------------------------------------------------------------------
// Map all feature endpoints (auto-discovery)
// ---------------------------------------------------------------------------
app.MapFeatureEndpoints();

app.Run();

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "Json Web Token"
                }
            };

            if (document.Paths is null) return;
#pragma warning disable CS8603
            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
#pragma warning restore CS8603
            {
                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }
        }
    }
}
