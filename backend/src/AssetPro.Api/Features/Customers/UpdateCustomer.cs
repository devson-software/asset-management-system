using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Customers;

public static class UpdateCustomer
{
    public record Request(
        Guid Id,
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
            RuleFor(x => x.Id).NotEmpty();
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

            var rows = await conn.ExecuteAsync("""
                UPDATE Customers SET
                    Name = @Name, ContactName = @ContactName, Email = @Email,
                    Mobile = @Mobile, Telephone = @Telephone, Address = @Address,
                    BillingAddress = @BillingAddress, VatNumber = @VatNumber,
                    PictureUrl = @PictureUrl,
                    UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UpdatedBy
                WHERE Id = @Id AND TenantId = @TenantId AND IsDeleted = 0
                """, new
            {
                request.Id,
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
                UpdatedBy = request.AuditUserId
            });

            if (rows == 0)
                throw new NotFoundException("Customer", request.Id);

            return new Response(request.Id, request.Name);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPut("/api/customers/{id:guid}", async (Guid id, UpdateCustomerBody body, ISender sender) =>
            Results.Ok(await sender.Send(new Request(
                id, body.Name, body.ContactName, body.Email, body.Mobile,
                body.Telephone, body.Address, body.BillingAddress, body.VatNumber, body.PictureUrl))))
        .RequireAuthorization()
        .WithTags("Customers");

    public record UpdateCustomerBody(
        string Name, string ContactName, string Email, string Mobile,
        string? Telephone, string Address, string? BillingAddress, string? VatNumber, string? PictureUrl);
}
