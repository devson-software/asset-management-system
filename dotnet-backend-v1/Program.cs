using DotnetBackendV1.Domain;
using DotnetBackendV1.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddSingleton<InMemoryData>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Simple health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTimeOffset.UtcNow }))
   .WithName("Health")
   .WithTags("System");

// Asset APIs
app.MapGet("/api/assets", (InMemoryData db) =>
    Results.Ok(db.GetAllAssets()))
   .WithName("GetAllAssets")
   .WithTags("Assets");

app.MapGet("/api/assets/{id}", (string id, InMemoryData db) =>
{
    var asset = db.GetAssetById(id);
    return asset is null ? Results.NotFound() : Results.Ok(asset);
})
.WithName("GetAssetById")
.WithTags("Assets");

app.MapGet("/api/customers/{customerId}/projects/{projectId}/assets",
    (string customerId, string projectId, InMemoryData db) =>
        Results.Ok(db.GetAssetsForProject(customerId, projectId)))
   .WithName("GetAssetsForProject")
   .WithTags("Assets");

app.MapPost("/api/customers/{customerId}/projects/{projectId}/assets",
    (string customerId, string projectId, Asset asset, InMemoryData db) =>
    {
        var created = db.AddAsset(customerId, projectId, asset);
        return Results.Created($"/api/assets/{created.Id}", created);
    })
   .WithName("CreateAsset")
   .WithTags("Assets");

app.MapPut("/api/assets/{id}", (string id, Asset asset, InMemoryData db) =>
{
    var updated = db.UpdateAsset(id, asset);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
})
.WithName("UpdateAsset")
.WithTags("Assets");

app.MapDelete("/api/assets/{id}", (string id, InMemoryData db) =>
{
    var removed = db.DeleteAsset(id);
    return removed ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteAsset")
.WithTags("Assets");

app.Run();
