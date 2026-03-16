# Architecture

Infinity Growth is organized as a layered .NET solution to keep responsibilities clear and make the codebase easier to evolve.

## High-level flow

```mermaid
flowchart LR
  Client[Client / UI] --> API[InfinityGrowth_Proyecto2 (ASP.NET Core API)]
  API --> Logic[AppLogic (Managers / Services)]
  Logic --> DA[DataAccess (CRUD / DAO / Mappers)]
  DA --> DB[(SQL Server via Stored Procedures)]
  Logic --> PayPal[PayPal SDK]
  Logic --> Twelve[TwelveData API]
  Logic --> ACS[Azure Communication Services Email]
  API --> DTO[DTO (Contracts)]
  Logic --> DTO
  DA --> DTO
```

## Projects

- `InfinityGrowth_Proyecto2` (API)
  - Controllers, middleware, DI composition root (`Program.cs`).
  - Global exception handling + model validation filter.

- `AppLogic` (application layer)
  - Managers and services orchestrate use-cases.
  - Integrations: PayPal, TwelveData, Azure Communication Services.

- `DataAccess` (data layer)
  - `ISqlDao` + `SqlDao` execute stored procedures.
  - `Crud` classes group database operations by feature/aggregate.
  - `Mappers` translate between DB rows and DTOs.

- `DTO` (contracts)
  - Shared request/response models used across layers.

## Design decisions (portfolio notes)

- **Layered architecture**: makes controllers thin and keeps business logic out of the transport layer.
- **DTO project**: keeps contracts stable and avoids circular dependencies.
- **Constructor injection**: improves testability and keeps dependencies explicit.
- **Secret management**: no secrets in the repo; configuration comes from env vars or gitignored local settings.

