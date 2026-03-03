using System.Reflection;

namespace AssetPro.Api.Common.Extensions;

public static class EndpointExtensions
{
    public static void MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var endpointTypes = assembly.GetTypes()
            .Where(t => t.GetMethod("Map", BindingFlags.Public | BindingFlags.Static,
                [typeof(IEndpointRouteBuilder)]) is not null);

        foreach (var type in endpointTypes)
        {
            var method = type.GetMethod("Map", BindingFlags.Public | BindingFlags.Static,
                [typeof(IEndpointRouteBuilder)]);
            method?.Invoke(null, [app]);
        }
    }
}
