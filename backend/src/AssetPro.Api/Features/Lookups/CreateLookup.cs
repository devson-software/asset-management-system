using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Lookups;

public static class CreateLookup
{
    public record Request(
        string Category,
        string Code,
        string DisplayName,
        int SortOrder = 0) : IRequest<Response>, ITenantRequest, IAuditedRequest
    {
        public Guid TenantId { get; set; }
        public Guid AuditUserId { get; set; }
    }

    public record Response(Guid Id, string Category, string Code, string DisplayName);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50)
                .Matches("^[a-z0-9_]+$").WithMessage("Code must be lowercase alphanumeric with underscores.");
            RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
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
                INSERT INTO ref.Lookups (Id, TenantId, Category, Code, DisplayName, SortOrder, CreatedAt, CreatedBy)
                VALUES (@Id, @TenantId, @Category, @Code, @DisplayName, @SortOrder, SYSUTCDATETIME(), @CreatedBy)
                """, new
            {
                Id = id,
                request.TenantId,
                request.Category,
                request.Code,
                request.DisplayName,
                request.SortOrder,
                CreatedBy = request.AuditUserId
            });

            return new Response(id, request.Category, request.Code, request.DisplayName);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/lookups", async (Request req, ISender sender) =>
        {
            var result = await sender.Send(req);
            return Results.Created($"/api/lookups/{result.Category}", result);
        })
        .RequireAuthorization()
        .WithTags("Lookups");
}
