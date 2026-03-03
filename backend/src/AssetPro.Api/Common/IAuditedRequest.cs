namespace AssetPro.Api.Common;

public interface IAuditedRequest
{
    Guid AuditUserId { get; set; }
}
