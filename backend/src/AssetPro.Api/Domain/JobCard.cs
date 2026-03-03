namespace AssetPro.Api.Domain;

public class JobCard : IAuditableEntity, ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string JobNumber { get; set; } = default!;
    public Guid AssetId { get; set; }
    public Guid? ServiceEventId { get; set; }
    public Guid TechnicianId { get; set; }
    public DateTime Date { get; set; }
    public string WorkType { get; set; } = "maintenance";
    public string? CheckInTime { get; set; }
    public string? CheckOutTime { get; set; }
    public bool FaultFound { get; set; }
    public string? FaultReported { get; set; }
    public string? RootCause { get; set; }
    public string? Remedy { get; set; }
    public string? Comments { get; set; }
    public string? ReadingsJson { get; set; }
    public string? ChecklistJson { get; set; }
    public string? SignatureUrl { get; set; }
    public string? SignedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
