## Contributing

Thanks for your interest in contributing.

### Local development

- Install **.NET SDK 8.x**
- Create local configuration (see `README.md` and `InfinityGrowth_Proyecto2/appsettings.Example.json`)
- Run the API from `InfinityGrowth_Proyecto2`

### Guidelines

- **Do not commit secrets** (API keys, connection strings, JWT keys). Use environment variables or local `appsettings.Development.json` (gitignored).
- Keep changes small and focused.
- Prefer constructor injection over `new` to keep code testable.

### Pull requests

- Include a clear description of the change and why it’s needed.
- Include a basic test plan (how you verified the change).

