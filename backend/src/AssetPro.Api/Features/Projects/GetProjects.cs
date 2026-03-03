using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Projects;

public static class GetProjects
{
    public record Request(Guid? CustomerId = null) : IRequest<IEnumerable<Response>>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    public record Response(
        Guid Id,
        Guid CustomerId,
        string CustomerName,
        string Name,
        string? SiteAddress,
        string? VendorLocation,
        string? TimeAllocation,
        string? PictureUrl,
        int AssetCount);

    internal class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            var sql = """
                SELECT p.Id, p.CustomerId, c.Name AS CustomerName, p.Name, p.SiteAddress,
                       p.VendorLocation, p.TimeAllocation, p.PictureUrl,
                       (SELECT COUNT(1) FROM Assets a WHERE a.ProjectId = p.Id AND a.IsDeleted = 0) AS AssetCount
                FROM Projects p
                INNER JOIN Customers c ON c.Id = p.CustomerId AND c.IsDeleted = 0
                WHERE p.TenantId = @TenantId AND p.IsDeleted = 0
                """;

            if (request.CustomerId.HasValue)
                sql += " AND p.CustomerId = @CustomerId";

            sql += " ORDER BY p.Name";

            return await conn.QueryAsync<Response>(sql, new { request.TenantId, request.CustomerId });
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/projects", async (Guid? customerId, ISender sender) =>
            Results.Ok(await sender.Send(new Request(customerId))))
        .RequireAuthorization()
        .WithTags("Projects");
}
