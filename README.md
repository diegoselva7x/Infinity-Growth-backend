# Infinity Growth – Investment Platform Backend

Infinity Growth is a .NET 8 backend API for an investment platform that manages users, advisors, wallets, portfolios, transactions, reporting and PayPal payouts.  
It is structured as a multi-project solution following a layered / clean-ish architecture: `InfinityGrowth_Proyecto2` (API), `AppLogic` (domain & services), `DataAccess` (DAO/CRUD/mappers), and `DTO` (contracts).

---

## Problem the project solves

- **Retail investors** need a way to manage portfolios, execute trades and view performance reports.
- **Advisors** need tools to see assigned clients, adjust commissions, and review client activity.
- **Administrators** need visibility into global metrics and the ability to manage platform configuration.

This backend exposes APIs to:

- Manage users, roles (admin / advisor / client) and authentication (JWT).
- Manage wallets, portfolios and active investments.
- Record and query transactions.
- Generate admin/ advisor / client reports.
- Integrate with TwelveData for market prices.
- Integrate with PayPal for withdrawals.

---

## Architecture overview

- **API layer**: `InfinityGrowth_Proyecto2`
  - ASP.NET Core 8 Web API project.
  - Controllers in `InfinityGrowth_Proyecto2/Controllers` expose HTTP endpoints.
  - Configures JWT authentication, CORS, Swagger, DI for managers/services.

- **Application layer**: `AppLogic`
  - Application services and managers (`*Manager` and `*Service` classes).
  - Orchestrates business rules, validation, and calls to `DataAccess`.
  - Handles integration with external services (TwelveData, PayPal, Azure Communication Services).

- **Data access layer**: `DataAccess`
  - `Dao`: low-level `SqlDao` and `SqlOperation` for stored procedure execution.
  - `Crud`: `*Crud` classes encapsulate CRUD operations per aggregate.
  - `Mappers`: translate between raw DB rows and DTOs.

- **Contracts layer**: `DTO`
  - Request/response DTOs for controllers and managers.
  - Report DTOs, PayPal DTOs, OTP DTOs, etc.

This separation keeps controllers thin, centralizes business logic, and isolates data access details.

---

## Tech stack

- **Runtime**: .NET 8
- **API**: ASP.NET Core Web API
- **Data access**: `Microsoft.Data.SqlClient` with stored procedures
- **Auth**: JWT (ASP.NET Core `JwtBearer`)
- **Security / crypto**: `BCrypt.Net-Next` for password hashing
- **External services**:
  - TwelveData (market data)
  - PayPal Checkout SDK for payouts
  - Azure Communication Services (email)
- **Documentation / tooling**:
  - Swagger / Swashbuckle

---

## Features

- **Authentication & authorization**
  - JWT-based authentication.
  - Role-based policies: Admin, Advisor, Client.

- **User & advisor management**
  - Registration, login, password recovery.
  - Advisor–client relationship management.
  - Commission adjustments and catalog configuration.

- **Wallet & portfolio**
  - Wallet balance and transactions.
  - Portfolios and active investments.
  - Buy/sell flows using TwelveData price feeds.

- **Reporting**
  - Client, advisor, and admin reports (summary and detail).

- **Integrations**
  - TwelveData for quotes and time series.
  - PayPal for transfers from wallet to PayPal.
  - Azure Communication Services for transactional emails.

---

## API capabilities (high level)

Controllers under `InfinityGrowth_Proyecto2/Controllers` expose endpoints such as:

- `AuthController` – login, token issuance.
- `UsuariosController` – user management.
- `WalletController` – wallet balance & operations.
- `TransaccionesController` – transaction history.
- `PortafoliosController` / `InversionesActivasController` – portfolio & investments.
- `ReporteController` – client, advisor, admin reports.
- `AsesoresController` / `RelacionAsesorClienteController` – advisor & client relationship management.
- `AjusteComisionesController` – commission adjustments.
- `PayPalController` – PayPal transfers.
- `CommunicationsController` – email sending.

Swagger is enabled so you can inspect all endpoints and schemas at runtime.

---

## Project structure

```text
Infinity-Growth-master/
  AppLogic/              # Application services, managers, integrations
  DataAccess/            # DAO, CRUDs, mappers to DB
  DTO/                   # DTO contracts used across layers
  InfinityGrowth_Proyecto2/  # ASP.NET Core Web API project (controllers, Program.cs, appsettings)
```

This layout is suitable for explaining layered / clean architecture experience in a portfolio.

---

## Setup instructions

### 1. Clone and open

- Clone the repository.
- Open the solution in Visual Studio or your preferred .NET IDE.

### 2. Configure appsettings

- Use `InfinityGrowth_Proyecto2/appsettings.Example.json` as a reference.
- Create your own **local** config file:
  - `InfinityGrowth_Proyecto2/appsettings.json` (will not be committed – see `.gitignore`), or
  - `InfinityGrowth_Proyecto2/appsettings.Development.json`.
- Fill in connection strings, API keys, and other values using your own infrastructure.

> **Important**: Real `appsettings*.json` files are gitignored. Only the `*.Example.json` templates are committed.

### 3. Set environment variables (recommended)

You can configure everything purely via environment variables (preferred for production-like setups).

#### Database (used by `DataAccess/Dao/SqlDao.cs`)

Set **one** of:

- `IG_DB_CONNECTIONSTRING` (**preferred**)
- `Azure__ConnectionStrings__DefaultConnectionDB`
- `ConnectionStrings__DefaultConnectionDB`

#### JWT (API auth)

- `JwtSettings__Key` – strong random secret.
- Optional overrides:
  - `JwtSettings__Issuer`
  - `JwtSettings__Audience`

#### TwelveData

- `TwelveData__ApiKey`

#### PayPal

- `PayPal__ClientId`
- `PayPal__ClientSecret`
- `PayPal__Environment` (e.g. `Sandbox` or `Live`)

#### Azure Communication Services email

- `Azure__ConnectionStrings__DefaultConnectionEmail`
- `Azure__ConnectionStrings__SenderAddressEmail`

---

## How to run locally

From the `InfinityGrowth_Proyecto2` folder:

```bash
dotnet restore
dotnet run
```

Or press **F5** in Visual Studio with `InfinityGrowth_Proyecto2` set as the startup project.

Once running:

- Swagger UI is available at the root path (e.g. `https://localhost:5001/`).
- Use Swagger to explore and test endpoints.

---

## Configuration best practices

- **Do not commit secrets**:
  - Use environment variables for production secrets.
  - Use local `appsettings.Development.json` for development only (gitignored).
- **Use the example files**:
  - `appsettings.Example.json` documents all required keys and value shapes.
- **JWT key management**:
  - Always use a long, random key.
  - Rotate the key when necessary.

---

## Security notes

- Passwords are stored using **BCrypt** hashing (`Encrypt_Service`).
- JWT authentication is configured with issuer, audience, and signing key validation.
- Database connection strings and external service keys are loaded from **environment variables** or local, gitignored configs.
- PayPal and TwelveData access keys are not committed; they must be provided by the operator.

For contributors, see `SECURITY.md` for policies on secret handling and vulnerability reporting.

---

## Design & architecture notes

- **Layered architecture**:
  - Controllers → Managers/Services → CRUD/DAO → Database.
- **Separation of concerns**:
  - DTOs are isolated in their own project.
  - Data access is centralized in `DataAccess`.
  - Business orchestration and external integrations live in `AppLogic`.
- **Extensibility**:
  - New features typically require:
    - New DTOs
    - A new manager/service
    - CRUD + mapper updates (if DB changes)
    - A new controller or endpoint method

---

## Testing & quality (roadmap)

While automated tests are not yet included, a professional next step would be:

- **Unit tests** for:
  - Managers (business rules).
  - Services (PayPal, TwelveData, email) with mocked HTTP/SDK dependencies.
- **Integration tests**:
  - API endpoints using an in-memory or test database.
- **Static analysis & style**:
  - Enforce consistent naming and style with analyzers / editorconfig.

---

## Future improvements / roadmap

- Add automated tests for core flows (auth, wallet operations, reporting).
- Introduce a more explicit **domain layer** to move toward a stricter clean architecture.
- Introduce a configuration abstraction over raw environment access for easier testing.
- Improve validation and error handling for all public endpoints.
- Add rate limiting and request logging for security and observability.

---

## Known limitations

- Depends on stored procedures and a specific SQL schema (not included here).
- Assumes availability of TwelveData, PayPal, and Azure Communication resources.
- Some controller actions rely on infrastructure details that should be abstracted further in a production system.

---

## Portfolio positioning

This project can be presented as:

- A **multi-service .NET backend** with:
  - Clean layering (API / AppLogic / DataAccess / DTO).
  - Real-world integrations (market data, email, payments).
  - Production-conscious security practices (JWT, BCrypt, secret handling).

Use this repository to demonstrate experience with:

- Designing and structuring a non-trivial .NET API.
- Working with external APIs (PayPal, TwelveData, Azure).
- Handling security-sensitive configuration in a way suitable for open source.
