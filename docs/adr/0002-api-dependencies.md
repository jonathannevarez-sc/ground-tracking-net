# 0002. API and test dependencies

Date: 2026-10-08

Status: Accepted for story 1478.

## Decision

Use Microsoft.Data.SqlClient 6.1.3 to connect the repository implementation to SQL Server. Use xUnit 2.9.3, xunit.runner.visualstudio 2.8.2, and Microsoft.NET.Test.Sdk 17.13.0 for repeatable unit tests with `dotnet test`.

| Package | Version | License | Reason |
|---|---:|---|---|
| Microsoft.Data.SqlClient | 6.1.3 | MIT | .NET does not provide a built-in SQL Server ADO.NET driver. |
| xunit | 2.9.3 | Apache-2.0 | The app needs a unit test framework that integrates with `dotnet test`. |
| xunit.runner.visualstudio | 2.8.2 | Apache-2.0 | Discovers and runs xUnit tests through the .NET test platform. |
| Microsoft.NET.Test.Sdk | 17.13.0 | MIT | Provides the .NET test host and test-platform integration. |

## Consequences

- Keep package versions explicit in project files and review updates.
- The SQL Server connection string is supplied through runtime configuration, not source control.
- Tests of payload validation do not require a database or network connection.
