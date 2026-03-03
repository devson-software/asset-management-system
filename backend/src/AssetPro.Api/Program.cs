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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not configured.");

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
// Run DbUp migrations
// ---------------------------------------------------------------------------
var migrationResult = DatabaseMigrator.Migrate(connectionString);
if (!migrationResult.Successful)
{
    app.Logger.LogError(migrationResult.Error, "Database migration failed.");
    throw migrationResult.Error;
}

// ---------------------------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------------------------
app.UseExceptionHandler();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// ---------------------------------------------------------------------------
// Health check
// ---------------------------------------------------------------------------
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }))
    .WithTags("System")
    .AllowAnonymous();

// ---------------------------------------------------------------------------
// Map all feature endpoints (auto-discovery)
// ---------------------------------------------------------------------------
app.MapFeatureEndpoints();

app.Run();
