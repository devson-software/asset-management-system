using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using AssetPro.Api.Infrastructure.Auth;

namespace AssetPro.Api.Features.Auth;

public static class Login
{
    public record Request(string Email, string Password) : IRequest<Response>;

    public record Response(string AccessToken, string RefreshToken, string FullName, string Role, Guid TenantId);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public Handler(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var valid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Store refresh token (simplified — in production, persist to DB with expiry)
            user.SecurityStamp = refreshToken;
            await _userManager.UpdateAsync(user);

            return new Response(accessToken, refreshToken, user.FullName, user.Role, user.TenantId);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/auth/login", async (Request req, ISender sender) =>
        {
            try
            {
                var result = await sender.Send(req);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .WithTags("Auth")
        .AllowAnonymous();
}
