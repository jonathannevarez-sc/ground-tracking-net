# Ground routes API

ASP.NET Core REST API for creating and reading ground routes. Route content remains an opaque JSON object because the API contract does not define route fields.

## API

- `POST /api/ground-routes` creates a route from a non-empty JSON object and returns `201 Created` with the saved route and its identifier.
- `GET /api/ground-routes/{id}` returns the route or `404 Not Found`.
- `GET /api/ground-routes` returns all routes, or an empty JSON array when none exist.
- An empty object, a JSON array, or `null` is rejected with `400 Bad Request`.

The service assigns a GUID identifier. It does not provide route updates or deletion.

## SQL Server

Create the `dbo.GroundRoutes` table by running `src/GroundRoutes.Api/schema.sql` against the target database. Configure the SQL Server connection string in the runtime environment as `ConnectionStrings__GroundRoutes`; do not commit credentials.

## Build and tests

Run `dotnet build` and `dotnet test` from the repository root.
