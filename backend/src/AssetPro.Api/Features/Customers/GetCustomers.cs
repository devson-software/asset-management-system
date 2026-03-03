using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Customers;

public static class GetCustomers
{
    public record Request : IRequest<IEnumerable<Response>>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    public record Response(
        Guid Id,
        string Name,
        string ContactName,
        string Email,
        string Mobile,
        string? Telephone,
        string Address,
        string? PictureUrl);

    internal class Handler : IRequestHandler<Request, IEnumerable<Response>>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            return await conn.QueryAsync<Response>("""
                SELECT Id, Name, ContactName, Email, Mobile, Telephone, Address, PictureUrl
                FROM app.Customers
                WHERE TenantId = @TenantId AND IsDeleted = 0
                ORDER BY Name
                """, new { request.TenantId });
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/customers", async (ISender sender) =>
            Results.Ok(await sender.Send(new Request())))
        .RequireAuthorization()
        .WithTags("Customers");
}
