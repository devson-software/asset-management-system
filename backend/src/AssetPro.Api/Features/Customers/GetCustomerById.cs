using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Customers;

public static class GetCustomerById
{
    public record Request(Guid Id) : IRequest<Response>, ITenantRequest
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
        string? BillingAddress,
        string? VatNumber,
        string? PictureUrl,
        DateTime CreatedAt);

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            var customer = await conn.QuerySingleOrDefaultAsync<Response>("""
                SELECT Id, Name, ContactName, Email, Mobile, Telephone,
                       Address, BillingAddress, VatNumber, PictureUrl, CreatedAt
                FROM Customers
                WHERE Id = @Id AND TenantId = @TenantId AND IsDeleted = 0
                """, new { request.Id, request.TenantId });

            return customer ?? throw new NotFoundException("Customer", request.Id);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/customers/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new Request(id))))
        .RequireAuthorization()
        .WithTags("Customers");
}
