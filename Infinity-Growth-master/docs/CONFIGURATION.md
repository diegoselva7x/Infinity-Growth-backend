# Configuration (safe for public GitHub)

This repository is intentionally configured to be **safe for open source**:

- Real `appsettings*.json` files are **gitignored**
- Example templates are committed:
  - `InfinityGrowth_Proyecto2/appsettings.Example.json`
  - `InfinityGrowth_Proyecto2/appsettings.Development.Example.json`

## Required environment variables

### Database

Set one of:

- `IG_DB_CONNECTIONSTRING` (preferred)
- `Azure__ConnectionStrings__DefaultConnectionDB`
- `ConnectionStrings__DefaultConnectionDB`

### JWT

- `JwtSettings__Key`
- Optional:
  - `JwtSettings__Issuer`
  - `JwtSettings__Audience`

### Integrations

- TwelveData: `TwelveData__ApiKey`
- PayPal:
  - `PayPal__ClientId`
  - `PayPal__ClientSecret`
  - `PayPal__Environment` (`Sandbox` / `Live`)
- Azure Communication Services Email:
  - `Azure__ConnectionStrings__DefaultConnectionEmail`
  - `Azure__ConnectionStrings__SenderAddressEmail`

## Local development config

You can also create:

- `InfinityGrowth_Proyecto2/appsettings.Development.json` (gitignored)

ASP.NET Core will merge configuration from:

- `appsettings.json`
- `appsettings.{Environment}.json`
- environment variables

Environment variables win by default.

