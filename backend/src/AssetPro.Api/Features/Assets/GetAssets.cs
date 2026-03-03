using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Assets;

public static class GetAssets
{
    public record Request(Guid? ProjectId = null) : IRequest<IEnumerable<Response>>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    public record Response(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        string CustomerName,
        string UnitRef,
        string? PlantCategoryName,
        string? UnitTypeName,
        string Manufacturer,
        string IndoorModel,
        string SerialNumber,
        string ServiceSchedule,
        string Status);

    internal class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            var sql = """
                SELECT a.Id, a.ProjectId, p.Name AS ProjectName, c.Name AS CustomerName,
                       a.UnitRef, ph_cat.Name AS PlantCategoryName, ph_type.Name AS UnitTypeName,
                       a.Manufacturer, a.IndoorModel,
                       a.SerialNumber, a.ServiceSchedule, a.Status
                FROM app.Assets a
                INNER JOIN app.Projects p ON p.Id = a.ProjectId AND p.IsDeleted = 0
                INNER JOIN app.Customers c ON c.Id = p.CustomerId AND c.IsDeleted = 0
                LEFT JOIN ref.PlantHierarchy ph_cat ON ph_cat.Id = a.PlantCategoryId
                LEFT JOIN ref.PlantHierarchy ph_type ON ph_type.Id = a.UnitTypeId
                WHERE a.TenantId = @TenantId AND a.IsDeleted = 0
                """;

            if (request.ProjectId.HasValue)
                sql += " AND a.ProjectId = @ProjectId";

            sql += " ORDER BY a.UnitRef";

            return await conn.QueryAsync<Response>(sql, new { request.TenantId, request.ProjectId });
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/assets", async (Guid? projectId, ISender sender) =>
            Results.Ok(await sender.Send(new Request(projectId))))
        .RequireAuthorization()
        .WithTags("Assets");
}
