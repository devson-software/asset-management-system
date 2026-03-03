namespace AssetPro.Api.Domain;

public class Lookup : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Category { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
