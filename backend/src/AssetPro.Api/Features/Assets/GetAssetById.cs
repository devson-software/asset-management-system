using Dapper;
using MediatR;
using AssetPro.Api.Common;
using AssetPro.Api.Common.Exceptions;
using AssetPro.Api.Infrastructure.Database;

namespace AssetPro.Api.Features.Assets;

public static class GetAssetById
{
    public record Request(Guid Id) : IRequest<Response>, ITenantRequest
    {
        public Guid TenantId { get; set; }
    }

    public record Response(
        Guid Id,
        Guid ProjectId,
        string ProjectName,
        string CustomerName,
        string UnitRef,
        Guid? PlantCategoryId,
        string? PlantCategoryName,
        Guid? UnitTypeId,
        string? UnitTypeName,
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
        string Status,
        string? NameplatePhotoUrl,
        string? QrCodeUrl,
        DateTime CreatedAt);

    internal class Handler : IRequestHandler<Request, Response>
    {
        private readonly IDbConnectionFactory _db;

        public Handler(IDbConnectionFactory db) => _db = db;

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            using var conn = await _db.CreateConnectionAsync(cancellationToken);

            var asset = await conn.QuerySingleOrDefaultAsync<Response>("""
                SELECT a.Id, a.ProjectId, p.Name AS ProjectName, c.Name AS CustomerName,
                       a.UnitRef, a.PlantCategoryId, ph_cat.Name AS PlantCategoryName,
                       a.UnitTypeId, ph_type.Name AS UnitTypeName,
                       a.Manufacturer, a.IndoorModel,
                       a.SerialNumber, a.OutdoorModel, a.OutdoorSerial, a.InstallationDate,
                       a.RefrigerantType, a.RefrigerantKg, a.ServiceSchedule, a.ServiceDuration,
                       a.VendorArea, a.VendorLocation, a.VendorAddress, a.Status,
                       a.NameplatePhotoUrl, a.QrCodeUrl, a.CreatedAt
                FROM Assets a
                INNER JOIN Projects p ON p.Id = a.ProjectId AND p.IsDeleted = 0
                INNER JOIN Customers c ON c.Id = p.CustomerId AND c.IsDeleted = 0
                LEFT JOIN PlantHierarchy ph_cat ON ph_cat.Id = a.PlantCategoryId
                LEFT JOIN PlantHierarchy ph_type ON ph_type.Id = a.UnitTypeId
                WHERE a.Id = @Id AND a.TenantId = @TenantId AND a.IsDeleted = 0
                """, new { request.Id, request.TenantId });

            return asset ?? throw new NotFoundException("Asset", request.Id);
        }
    }

    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/api/assets/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new Request(id))))
        .RequireAuthorization()
        .WithTags("Assets");
}
