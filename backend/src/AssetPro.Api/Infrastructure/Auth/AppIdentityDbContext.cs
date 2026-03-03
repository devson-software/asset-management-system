using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetPro.Api.Infrastructure.Auth;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            b.Property(u => u.Role).HasMaxLength(30).IsRequired();
            b.Property(u => u.InvitationStatus).HasMaxLength(30).IsRequired();
            b.Property(u => u.PictureUrl).HasMaxLength(500);
            b.HasIndex(u => u.TenantId);
        });
    }
}
