using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Lookups;

public static class GetLookups
{
    public record Request(string Category) : IRequest<IEnumerable<Response>>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    public record Response(Guid Id, string Code, string DisplayName, int SortOrder, bool IsSystem);

    internal class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            return await conn.QueryAsync<Response>("""
                SELECT Id, Code, DisplayName, SortOrder,
                       CASE WHEN TenantId IS NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsSystem
                FROM ref.Lookups
                WHERE Category = @Category
                  AND (TenantId IS NULL OR TenantId = @TenantId)
                  AND IsDeleted = 0 AND IsActive = 1
                ORDER BY SortOrder, DisplayName
                """, new { request.Category, request.TenantId });
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/lookups/{category}", async (string category, ISender sender) =>
            Results.Ok(await sender.Send(new Request(category))))
        .RequireAuthorization()
        .WithTags("Lookups");
}
