using System.Text.Json;
using GroundRoutes.Api.Models;

namespace GroundRoutes.Api.Data;

public interface IGroundRouteRepository
{
    Task<GroundRoute> CreateAsync(JsonElement data, CancellationToken cancellationToken);

    Task<GroundRoute?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<GroundRoute>> GetAllAsync(CancellationToken cancellationToken);
}
