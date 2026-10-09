using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GroundRoutes.Api.Data;
using GroundRoutes.Api.Endpoints;
using GroundRoutes.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GroundRoutes.Api.Tests;

public sealed class GroundRouteAcceptanceTests
{
    [Fact]
    public async Task test_tc1483_ac1_create_returns_saved_route_details()
    {
        var repository = new InMemoryGroundRouteRepository();
        var (app, client) = await StartApiAsync(repository);
        await using (app)
        using (client)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/ground-routes",
                new { routeCode = "SYN-ROUTE-AC1" });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var route = await response.Content.ReadFromJsonAsync<GroundRoute>();
            Assert.NotNull(route);
            Assert.NotEqual(Guid.Empty, route.Id);
            Assert.Equal(
                "SYN-ROUTE-AC1",
                route.Data.GetProperty("routeCode").GetString());
            Assert.NotNull(response.Headers.Location);
        }
    }

    [Fact]
    public async Task test_tc1483_ac2_get_returns_existing_route_details()
    {
        var id = Guid.NewGuid();
        using var routeData = JsonDocument.Parse("{\"routeCode\":\"SYN-ROUTE-AC2\"}");
        var repository = new InMemoryGroundRouteRepository(
            new GroundRoute(id, routeData.RootElement.Clone()));
        var (app, client) = await StartApiAsync(repository);
        await using (app)
        using (client)
        {
            using var response = await client.GetAsync($"/api/ground-routes/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var route = await response.Content.ReadFromJsonAsync<GroundRoute>();
            Assert.NotNull(route);
            Assert.Equal(id, route.Id);
            Assert.Equal(
                "SYN-ROUTE-AC2",
                route.Data.GetProperty("routeCode").GetString());
        }
    }

    [Fact]
    public async Task test_tc1483_ac3_empty_route_data_is_rejected_without_creation()
    {
        var repository = new InMemoryGroundRouteRepository();
        var (app, client) = await StartApiAsync(repository);
        await using (app)
        using (client)
        {
            using var response = await client.PostAsJsonAsync(
                "/api/ground-routes",
                new { });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Empty(await repository.GetAllAsync(CancellationToken.None));
        }
    }

    [Fact]
    public async Task test_tc1483_ac4_list_returns_empty_when_no_routes_exist()
    {
        var repository = new InMemoryGroundRouteRepository();
        var (app, client) = await StartApiAsync(repository);
        await using (app)
        using (client)
        {
            using var response = await client.GetAsync("/api/ground-routes");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var routes = await response.Content.ReadFromJsonAsync<GroundRoute[]>();
            Assert.NotNull(routes);
            Assert.Empty(routes);
        }
    }

    private static async Task<(WebApplication App, HttpClient Client)> StartApiAsync(
        IGroundRouteRepository repository)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development"
        });
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddSingleton(repository);

        var app = builder.Build();
        app.MapGroundRouteEndpoints();
        await app.StartAsync();

        var server = app.Services.GetRequiredService<IServer>();
        var address = server.Features.Get<IServerAddressesFeature>()!
            .Addresses.Single();

        return (app, new HttpClient { BaseAddress = new Uri(address) });
    }

    private sealed class InMemoryGroundRouteRepository(
        params GroundRoute[] initialRoutes) : IGroundRouteRepository
    {
        private readonly List<GroundRoute> _routes = [.. initialRoutes];

        public Task<GroundRoute> CreateAsync(
            JsonElement data,
            CancellationToken cancellationToken)
        {
            var route = new GroundRoute(Guid.NewGuid(), data.Clone());
            _routes.Add(route);
            return Task.FromResult(route);
        }

        public Task<GroundRoute?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken) =>
            Task.FromResult(_routes.SingleOrDefault(route => route.Id == id));

        public Task<IReadOnlyList<GroundRoute>> GetAllAsync(
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<GroundRoute>>(_routes.ToArray());
    }
}
