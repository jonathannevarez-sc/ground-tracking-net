using System.Data;
using System.Text.Json;
using GroundRoutes.Api.Models;
using Microsoft.Data.SqlClient;

namespace GroundRoutes.Api.Data;

public sealed class SqlGroundRouteRepository(string connectionString)
    : IGroundRouteRepository
{
    public async Task<GroundRoute> CreateAsync(
        JsonElement data,
        CancellationToken cancellationToken)
    {
        var route = new GroundRoute(Guid.NewGuid(), data.Clone());

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO dbo.GroundRoutes (Id, RouteData) VALUES (@Id, @RouteData);";
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = route.Id;
        command.Parameters.Add("@RouteData", SqlDbType.NVarChar, -1).Value =
            route.Data.GetRawText();

        await command.ExecuteNonQueryAsync(cancellationToken);
        return route;
    }

    public async Task<GroundRoute?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT Id, RouteData FROM dbo.GroundRoutes WHERE Id = @Id;";
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return ReadRoute(reader);
    }

    public async Task<IReadOnlyList<GroundRoute>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var routes = new List<GroundRoute>();

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT Id, RouteData FROM dbo.GroundRoutes ORDER BY Id;";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            routes.Add(ReadRoute(reader));
        }

        return routes;
    }

    private static GroundRoute ReadRoute(SqlDataReader reader)
    {
        using var document = JsonDocument.Parse(reader.GetString(1));
        return new GroundRoute(reader.GetGuid(0), document.RootElement.Clone());
    }
}
