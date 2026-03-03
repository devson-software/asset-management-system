using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.PlantHierarchy;

public static class GetPlantHierarchy
{
    public record Request : IRequest<IEnumerable<Response>>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    public record Response(
        Guid Id,
        Guid? ParentId,
        string Name,
        int Level,
        int SortOrder,
        bool IsSystem);

    internal class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            return await conn.QueryAsync<Response>("""
                SELECT Id, ParentId, Name, Level, SortOrder,
                       CASE WHEN TenantId IS NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsSystem
                FROM PlantHierarchy
                WHERE (TenantId IS NULL OR TenantId = @TenantId)
                  AND IsDeleted = 0 AND IsActive = 1
                ORDER BY Level, SortOrder, Name
                """, new { request.TenantId });
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/plant-hierarchy", async (ISender sender) =>
            Results.Ok(await sender.Send(new Request())))
        .RequireAuthorization()
        .WithTags("PlantHierarchy");
}
