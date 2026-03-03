using MediatR;

namespace AssetPro.Api.Common.Behaviors;

public class TenantBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ITenantContext _tenantContext;

    public TenantBehavior(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ITenantRequest tenantRequest && tenantRequest.TenantId == Guid.Empty)
        {
            tenantRequest.TenantId = _tenantContext.TenantId;
        }

        return await next();
    }
}
