# 0001. Stack for service work

Date: 2026-10-08

Status: Accepted. The signed-in person confirmed build_setup for story 1478.

## Decision

This repo is a service project. ASP.NET Core REST API in C# with SQL Server persistence using the Repository Pattern, plus unit and integration tests.

| | |
|---|---|
| Language | C# |
| Framework | ASP.NET Core |
| Runtime | .NET |
| Test | `dotnet test` |
| Build | `dotnet build` |
| Template | none |

## Layout

- src/: API and application code
- tests/: unit and integration tests

## Consequences

- Every story in this repo follows `.mda/repo.json`. The build agent reads it before it writes code.
- A new package needs an ADR.
- The agent never deploys. The company pipeline builds, scans, and deploys.
- Changing the stack needs a new ADR and an edit to `.mda/repo.json`.
