namespace AssetPro.Api.Domain;

public class Asset : IAuditableEntity, ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProjectId { get; set; }
    public string UnitRef { get; set; } = default!;
    public string? PlantCategory { get; set; }
    public string UnitType { get; set; } = default!;
    public string Manufacturer { get; set; } = default!;
    public string IndoorModel { get; set; } = default!;
    public string SerialNumber { get; set; } = default!;
    public string? OutdoorModel { get; set; }
    public string? OutdoorSerial { get; set; }
    public DateTime? InstallationDate { get; set; }
    public string RefrigerantType { get; set; } = "R410A";
    public decimal? RefrigerantKg { get; set; }
    public string ServiceSchedule { get; set; } = "Monthly";
    public string? ServiceDuration { get; set; }
    public string? VendorArea { get; set; }
    public string? VendorLocation { get; set; }
    public string? VendorAddress { get; set; }
    public string Status { get; set; } = "Registered";
    public string? NameplatePhotoUrl { get; set; }
    public string? QrCodeUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
