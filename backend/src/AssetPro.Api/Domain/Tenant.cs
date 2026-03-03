namespace AssetPro.Api.Domain;

public class Tenant : IAuditableEntity
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = default!;
    public string Subdomain { get; set; } = default!;
    public string ContactEmail { get; set; } = default!;
    public string? ContactPhone { get; set; }
    public string? LogoUrl { get; set; }
    public string SubscriptionPlan { get; set; } = "Trial";
    public string SubscriptionStatus { get; set; } = "Active";
    public DateTime? SubscriptionExpiry { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
