namespace AssetPro.Api.Domain;

public class UnitLibraryItem : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Manufacturer { get; set; } = default!;
    public Guid? UnitTypeId { get; set; }
    public string IndoorModel { get; set; } = default!;
    public string? OutdoorModel { get; set; }
    public string RefrigerantType { get; set; } = default!;
    public decimal? RefrigerantCharge { get; set; }
    public string? ServiceDuration { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
