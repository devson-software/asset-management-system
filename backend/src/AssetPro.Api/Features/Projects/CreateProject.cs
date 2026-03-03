using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Projects;

public static class CreateProject
{
    public record Request(
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
            var id = Guid.NewGuid();

            await conn.ExecuteAsync("""
                INSERT INTO app.Projects (Id, TenantId, CustomerId, Name, SiteAddress,
                    VendorLocation, TimeAllocation, PictureUrl, CreatedAt, CreatedBy)
                VALUES (@Id, @TenantId, @CustomerId, @Name, @SiteAddress,
                    @VendorLocation, @TimeAllocation, @PictureUrl, SYSUTCDATETIME(), @CreatedBy)
                """, new
            {
                Id = id,
                request.TenantId,
                request.CustomerId,
                request.Name,
                request.SiteAddress,
                request.VendorLocation,
                request.TimeAllocation,
                request.PictureUrl,
                CreatedBy = request.AuditUserId
            });

            return new Response(id, request.Name);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/projects", async (Request req, ISender sender) =>
        {
            var result = await sender.Send(req);
            return Results.Created($"/api/projects/{result.Id}", result);
        })
        .RequireAuthorization()
        .WithTags("Projects");
}
