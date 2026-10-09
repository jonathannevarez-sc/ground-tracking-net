using System.Text.Json;

namespace GroundRoutes.Api.Models;

public sealed record GroundRoute(Guid Id, JsonElement Data);
