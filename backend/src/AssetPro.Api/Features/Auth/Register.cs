using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using AssetPro.Api.Infrastructure.Auth;
using AssetPro.Api.Infrastructure.Database;
using Dapper;

namespace AssetPro.Api.Features.Auth;

public static class Register
{
    public record Request(
        string CompanyName,
        string Subdomain,
        string FullName,
        string Email,
        string Password,
        string? ContactPhone) : IRequest<Response>;

    public record Response(Guid TenantId, Guid UserId, string AccessToken);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(300);
            RuleFor(x => x.Subdomain).NotEmpty().MaximumLength(100)
                .Matches("^[a-z0-9-]+$").WithMessage("Subdomain must be lowercase alphanumeric with hyphens only.");
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDbConnectionFactory _db;
        private readonly IJwtTokenService _jwtTokenService;

        public Handler(
            UserManager<ApplicationUser> userManager,
            IDbConnectionFactory db,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _db = db;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            // Check subdomain uniqueness
            var existing = await conn.QuerySingleOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Tenants WHERE Subdomain = @Subdomain AND IsDeleted = 0",
                new { request.Subdomain });

            if (existing > 0)
                throw new InvalidOperationException("Subdomain is already taken.");

            // Create tenant
            var tenantId = Guid.NewGuid();
            var systemUserId = Guid.Empty; // Will be updated after user creation

            await conn.ExecuteAsync("""
                INSERT INTO Tenants (Id, CompanyName, Subdomain, ContactEmail, ContactPhone,
                    SubscriptionPlan, SubscriptionStatus, CreatedAt, CreatedBy)
                VALUES (@Id, @CompanyName, @Subdomain, @Email, @ContactPhone,
                    'Trial', 'Active', SYSUTCDATETIME(), @CreatedBy)
                """, new
            {
                Id = tenantId,
                request.CompanyName,
                request.Subdomain,
                request.Email,
                request.ContactPhone,
                CreatedBy = systemUserId
            });

            // Create admin user
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                Role = "Admin",
                IsActive = true,
                InvitationStatus = "Accepted"
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            // Update tenant CreatedBy to the new user
            await conn.ExecuteAsync(
                "UPDATE Tenants SET CreatedBy = @UserId WHERE Id = @TenantId",
                new { UserId = user.Id, TenantId = tenantId });

            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            return new Response(tenantId, user.Id, accessToken);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/auth/register", async (Request req, ISender sender) =>
        {
            var result = await sender.Send(req);
            return Results.Created($"/api/tenants/{result.TenantId}", result);
        })
        .WithTags("Auth")
        .AllowAnonymous();
}
