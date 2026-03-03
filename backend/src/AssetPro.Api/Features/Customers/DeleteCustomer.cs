using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Customers;

public static class DeleteCustomer
{
    public record Request(Guid Id) : IRequest<Unit>, ITenantRequest, IAuditedRequest
    {
        public Guid TenantId { get; set; }
        public Guid AuditUserId { get; set; }
    }

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            var rows = await conn.ExecuteAsync("""
                UPDATE Customers SET
                    IsDeleted = 1, DeletedAt = SYSUTCDATETIME(), DeletedBy = @DeletedBy
                WHERE Id = @Id AND TenantId = @TenantId AND IsDeleted = 0
                """, new
            {
                request.Id,
                request.TenantId,
                DeletedBy = request.AuditUserId
            });

            if (rows == 0)
                throw new NotFoundException("Customer", request.Id);

            return Unit.Value;
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapDelete("/api/customers/{id:guid}", async (Guid id, ISender sender) =>
        {
            await sender.Send(new Request(id));
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Customers");
}
