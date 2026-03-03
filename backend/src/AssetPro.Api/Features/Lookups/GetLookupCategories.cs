using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Lookups;

public static class GetLookupCategories
{
    public record Request : IRequest<IEnumerable<string>>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    internal class Handler : IRequestHandler<Request, IEnumerable<string>>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<string>> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            return await conn.QueryAsync<string>("""
                SELECT DISTINCT Category
                FROM Lookups
                WHERE (TenantId IS NULL OR TenantId = @TenantId)
                  AND IsDeleted = 0
                ORDER BY Category
                """, new { request.TenantId });
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/lookups", async (ISender sender) =>
            Results.Ok(await sender.Send(new Request())))
        .RequireAuthorization()
        .WithTags("Lookups");
}
