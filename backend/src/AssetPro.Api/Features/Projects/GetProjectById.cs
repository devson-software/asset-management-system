using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Projects;

public static class GetProjectById
{
    public record Request(Guid Id) : IRequest<Response>, ITenantRequest
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
        DateTime CreatedAt);

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            var project = await conn.QuerySingleOrDefaultAsync<Response>("""
                SELECT p.Id, p.CustomerId, c.Name AS CustomerName, p.Name, p.SiteAddress,
                       p.VendorLocation, p.TimeAllocation, p.PictureUrl, p.CreatedAt
                FROM app.Projects p
                INNER JOIN app.Customers c ON c.Id = p.CustomerId AND c.IsDeleted = 0
                WHERE p.Id = @Id AND p.TenantId = @TenantId AND p.IsDeleted = 0
                """, new { request.Id, request.TenantId });

            return project ?? throw new NotFoundException("Project", request.Id);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/projects/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new Request(id))))
        .RequireAuthorization()
        .WithTags("Projects");
}
