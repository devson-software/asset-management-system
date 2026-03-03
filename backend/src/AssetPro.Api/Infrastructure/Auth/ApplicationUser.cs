using Microsoft.AspNetCore.Identity;

namespace AssetPro.Api.Infrastructure.Auth;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = "Technician";
    public bool IsActive { get; set; } = true;
    public string InvitationStatus { get; set; } = "Pending";
    public string? PictureUrl { get; set; }
}
