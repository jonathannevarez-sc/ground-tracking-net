using System.Text.Json;
using GroundRoutes.Api.Data;
using GroundRoutes.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GroundRoutes.Api.Endpoints;

public static class GroundRouteEndpoints
{
    public static IEndpointRouteBuilder MapGroundRouteEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var routes = endpoints.MapGroup("/api/ground-routes");

        routes.MapPost("/", CreateAsync);
        routes.MapGet("/{id:guid}", GetByIdAsync)
            .WithName("GetGroundRouteById");
        routes.MapGet("/", GetAllAsync);

        return endpoints;
    }

    private static async Task<Results<CreatedAtRoute<GroundRoute>, BadRequest<string>>>
        CreateAsync(
            JsonElement payload,
            IGroundRouteRepository repository,
            CancellationToken cancellationToken)
    {
        if (!GroundRoutePayloadValidator.IsValid(payload))
        {
            return TypedResults.BadRequest(
                "Route data must be a non-empty JSON object.");
        }

        var route = await repository.CreateAsync(payload, cancellationToken);
        return TypedResults.CreatedAtRoute(
            route,
            "GetGroundRouteById",
            new { id = route.Id });
    }

    private static async Task<Results<Ok<GroundRoute>, NotFound>> GetByIdAsync(
        Guid id,
        IGroundRouteRepository repository,
        CancellationToken cancellationToken)
    {
        var route = await repository.GetByIdAsync(id, cancellationToken);
        return route is null ? TypedResults.NotFound() : TypedResults.Ok(route);
    }

    private static async Task<Ok<IReadOnlyList<GroundRoute>>> GetAllAsync(
        IGroundRouteRepository repository,
        CancellationToken cancellationToken)
    {
        var routes = await repository.GetAllAsync(cancellationToken);
        return TypedResults.Ok(routes);
    }
}
