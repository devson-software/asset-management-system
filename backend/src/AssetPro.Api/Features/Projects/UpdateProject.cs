using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Projects;

public static class UpdateProject
{
    public record Request(
        Guid Id,
        Guid CustomerId,
        string Name,
        string? SiteAddress,
        string? VendorLocation,
        string? TimeAllocation,
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
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(300);
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
                UPDATE Projects SET
                    CustomerId = @CustomerId, Name = @Name, SiteAddress = @SiteAddress,
                    VendorLocation = @VendorLocation, TimeAllocation = @TimeAllocation,
                    PictureUrl = @PictureUrl,
                    UpdatedAt = SYSUTCDATETIME(), UpdatedBy = @UpdatedBy
                WHERE Id = @Id AND TenantId = @TenantId AND IsDeleted = 0
                """, new
            {
                request.Id,
                request.TenantId,
                request.CustomerId,
                request.Name,
                request.SiteAddress,
                request.VendorLocation,
                request.TimeAllocation,
                request.PictureUrl,
                UpdatedBy = request.AuditUserId
            });

            if (rows == 0)
                throw new NotFoundException("Project", request.Id);

            return new Response(request.Id, request.Name);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPut("/api/projects/{id:guid}", async (Guid id, UpdateProjectBody body, ISender sender) =>
            Results.Ok(await sender.Send(new Request(
                id, body.CustomerId, body.Name, body.SiteAddress,
                body.VendorLocation, body.TimeAllocation, body.PictureUrl))))
        .RequireAuthorization()
        .WithTags("Projects");

    public record UpdateProjectBody(
        Guid CustomerId, string Name, string? SiteAddress,
        string? VendorLocation, string? TimeAllocation, string? PictureUrl);
}
