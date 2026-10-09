using System.Text.Json;

namespace GroundRoutes.Api.Models;

public static class GroundRoutePayloadValidator
{
    public static bool IsValid(JsonElement payload) =>
        payload.ValueKind == JsonValueKind.Object &&
        payload.EnumerateObject().Any();
}
