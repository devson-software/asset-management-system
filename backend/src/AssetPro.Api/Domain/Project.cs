namespace AssetPro.Api.Domain;

public class Project : IAuditableEntity, ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; } = default!;
    public string? SiteAddress { get; set; }
    public string? VendorLocation { get; set; }
    public string? TimeAllocation { get; set; }
    public string? PictureUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
