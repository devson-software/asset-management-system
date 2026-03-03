using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AssetPro.Api.Common;

public class HttpUserContext : ICurrentUserContext, ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var sub = User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
        }
    }

    public Guid TenantId
    {
        get
        {
            var tenantClaim = User?.FindFirstValue("tenantId");
            return Guid.TryParse(tenantClaim, out var id) ? id : Guid.Empty;
        }
    }

    public string Role => User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
