using GroundRoutes.Api.Data;
using GroundRoutes.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GroundRoutes");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure the SQL Server connection with ConnectionStrings__GroundRoutes.");
}

builder.Services.AddSingleton<IGroundRouteRepository>(
    new SqlGroundRouteRepository(connectionString));

var app = builder.Build();

app.MapGroundRouteEndpoints();

app.Run();

public partial class Program;
