using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Lookups;

public static class UpdateLookup
{
    public record Request(
        Guid Id,
        string DisplayName,
        int SortOrder,
        bool IsActive) : IRequest<Response>, ITenantRequest, IAuditedRequest
    {
        public Guid TenantId { get; set; }
        public Guid AuditUserId { get; set; }
    }

    public record Response(Guid Id, string DisplayName);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
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

            // Only allow editing tenant-owned lookups (not system defaults)
            var rows = await conn.ExecuteAsync("""
                UPDATE Lookups SET
                    DisplayName = @DisplayName, SortOrder = @SortOrder, IsActive = @IsActive,
                    UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UpdatedBy
                WHERE Id = @Id AND TenantId = @TenantId AND IsDeleted = 0
                """, new
            {
                request.Id,
                request.TenantId,
                request.DisplayName,
                request.SortOrder,
                request.IsActive,
                UpdatedBy = request.AuditUserId
            });

            if (rows == 0)
                throw new NotFoundException("Lookup", request.Id);

            return new Response(request.Id, request.DisplayName);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPut("/api/lookups/{id:guid}", async (Guid id, UpdateLookupBody body, ISender sender) =>
            Results.Ok(await sender.Send(new Request(id, body.DisplayName, body.SortOrder, body.IsActive))))
        .RequireAuthorization()
        .WithTags("Lookups");

    public record UpdateLookupBody(string DisplayName, int SortOrder, bool IsActive);
}
