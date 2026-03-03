using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.PlantHierarchy;

public static class CreatePlantHierarchyItem
{
    public record Request(
        Guid? ParentId,
        string Name,
        int Level,
        int SortOrder = 0) : IRequest<Response>, ITenantRequest, IAuditedRequest
    {
        public Guid TenantId { get; set; }
        public Guid AuditUserId { get; set; }
    }

    public record Response(Guid Id, string Name, int Level);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Level).InclusiveBetween(0, 5);
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
                INSERT INTO PlantHierarchy (Id, TenantId, ParentId, Name, Level, SortOrder, CreatedAt, CreatedBy)
                VALUES (@Id, @TenantId, @ParentId, @Name, @Level, @SortOrder, SYSUTCDATETIME(), @CreatedBy)
                """, new
            {
                Id = id,
                request.TenantId,
                request.ParentId,
                request.Name,
                request.Level,
                request.SortOrder,
                CreatedBy = request.AuditUserId
            });

            return new Response(id, request.Name, request.Level);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/plant-hierarchy", async (Request req, ISender sender) =>
        {
            var result = await sender.Send(req);
            return Results.Created($"/api/plant-hierarchy/{result.Id}", result);
        })
        .RequireAuthorization()
        .WithTags("PlantHierarchy");
}
