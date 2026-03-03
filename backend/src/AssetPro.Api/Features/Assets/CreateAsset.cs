using Dapper;
using FluentValidation;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Assets;

public static class CreateAsset
{
    public record Request(
        Guid ProjectId,
        string UnitRef,
        Guid? PlantCategoryId,
        Guid? UnitTypeId,
        string Manufacturer,
        string IndoorModel,
        string SerialNumber,
        string? OutdoorModel,
        string? OutdoorSerial,
        DateTime? InstallationDate,
        string RefrigerantType,
        decimal? RefrigerantKg,
        string ServiceSchedule,
        string? ServiceDuration,
        string? VendorArea,
        string? VendorLocation,
        string? VendorAddress,
        string? NameplatePhotoUrl) : IRequest<Response>, ITenantRequest, IAuditedRequest
    {
        public Guid TenantId { get; set; }
        public Guid AuditUserId { get; set; }
    }

    public record Response(Guid Id, string UnitRef);

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProjectId).NotEmpty();
            RuleFor(x => x.UnitRef).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Manufacturer).NotEmpty().MaximumLength(200);
            RuleFor(x => x.IndoorModel).NotEmpty().MaximumLength(200);
            RuleFor(x => x.SerialNumber).NotEmpty().MaximumLength(100);
            RuleFor(x => x.RefrigerantType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ServiceSchedule).NotEmpty().MaximumLength(50);
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
                INSERT INTO Assets (Id, TenantId, ProjectId, UnitRef, PlantCategoryId, UnitTypeId,
                    Manufacturer, IndoorModel, SerialNumber, OutdoorModel, OutdoorSerial,
                    InstallationDate, RefrigerantType, RefrigerantKg, ServiceSchedule, ServiceDuration,
                    VendorArea, VendorLocation, VendorAddress, NameplatePhotoUrl, Status,
                    CreatedAt, CreatedBy)
                VALUES (@Id, @TenantId, @ProjectId, @UnitRef, @PlantCategoryId, @UnitTypeId,
                    @Manufacturer, @IndoorModel, @SerialNumber, @OutdoorModel, @OutdoorSerial,
                    @InstallationDate, @RefrigerantType, @RefrigerantKg, @ServiceSchedule, @ServiceDuration,
                    @VendorArea, @VendorLocation, @VendorAddress, @NameplatePhotoUrl, 'registered',
                    SYSUTCDATETIME(), @CreatedBy)
                """, new
            {
                Id = id,
                request.TenantId,
                request.ProjectId,
                request.UnitRef,
                request.PlantCategoryId,
                request.UnitTypeId,
                request.Manufacturer,
                request.IndoorModel,
                request.SerialNumber,
                request.OutdoorModel,
                request.OutdoorSerial,
                request.InstallationDate,
                request.RefrigerantType,
                request.RefrigerantKg,
                request.ServiceSchedule,
                request.ServiceDuration,
                request.VendorArea,
                request.VendorLocation,
                request.VendorAddress,
                request.NameplatePhotoUrl,
                CreatedBy = request.AuditUserId
            });

            return new Response(id, request.UnitRef);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/api/assets", async (Request req, ISender sender) =>
        {
            var result = await sender.Send(req);
            return Results.Created($"/api/assets/{result.Id}", result);
        })
        .RequireAuthorization()
        .WithTags("Assets");
}
