namespace AssetPro.Api.Common;

public interface ITenantContext
{
    Guid TenantId { get; }
}
