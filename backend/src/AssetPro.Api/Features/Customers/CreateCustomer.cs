using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Customers;

public static class CreateCustomer
{
    public record Request(
        string Name,
        string ContactName,
        string Email,
        string Mobile,
        string? Telephone,
        string Address,
        string? BillingAddress,
        string? VatNumber,
        string? PictureUrl) : IRequest<Response>, ITenantRequest, IAuditedRequest
    {
        public Guid TenantId { get; set; }
        public Guid AuditUserId { get; set; }
    }

    public record Response(Guid Id, string Name);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
            RuleFor(x => x.ContactName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Mobile).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        }
    }

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);
            var id = Guid.NewGuid();

            await conn.ExecuteAsync("""
                INSERT INTO Customers (Id, TenantId, Name, ContactName, Email, Mobile,
                    Telephone, Address, BillingAddress, VatNumber, PictureUrl, CreatedAt, CreatedBy)
                VALUES (@Id, @TenantId, @Name, @ContactName, @Email, @Mobile,
                    @Telephone, @Address, @BillingAddress, @VatNumber, @PictureUrl, SYSUTCDATETIME(), @CreatedBy)
                """, new
            {
                Id = id,
                request.TenantId,
                request.Name,
                request.ContactName,
                request.Email,
                request.Mobile,
                request.Telephone,
                request.Address,
                request.BillingAddress,
                request.VatNumber,
                request.PictureUrl,
                CreatedBy = request.AuditUserId
            });

            return new Response(id, request.Name);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/customers", async (Request req, ISender sender) =>
        {
            var result = await sender.Send(req);
            return Results.Created($"/api/customers/{result.Id}", result);
        })
        .RequireAuthorization()
        .WithTags("Customers");
}
