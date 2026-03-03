namespace AssetPro.Api.Domain;

public class CommissioningRecord : IAuditableEntity, ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? AssetId { get; set; }
    public string UnitRef { get; set; } = default!;
    public string CommissioningType { get; set; } = default!;
    public string Status { get; set; } = "in_progress";
    public string? FormDataJson { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
