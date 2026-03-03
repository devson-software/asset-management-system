namespace AssetPro.Api.Domain;

public class Customer : IAuditableEntity, ITenantScoped
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public string ContactName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Mobile { get; set; } = default!;
    public string? Telephone { get; set; }
    public string Address { get; set; } = default!;
    public string? BillingAddress { get; set; }
    public string? VatNumber { get; set; }
    public string? PictureUrl { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
