using MediatR;

namespace AssetPro.Api.Common.Behaviors;

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUserContext _currentUser;

    public AuditBehavior(ICurrentUserContext currentUser)
    {
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IAuditedRequest auditedRequest && auditedRequest.AuditUserId == Guid.Empty)
        {
            auditedRequest.AuditUserId = _currentUser.UserId;
        }

        return await next();
    }
}
