# Security Policy

## Reporting a vulnerability

If you discover a security issue (including accidentally committed secrets), please **do not open a public issue**. Instead, contact the repository owner privately and include:

- A clear description of the issue
- Steps to reproduce (if applicable)
- Impact assessment (what an attacker could do)

## Secret handling guidelines (contributors)

- Never commit credentials, connection strings with passwords, API keys, JWT signing keys, private keys, or certificates.
- Use environment variables for secrets.
- Use local-only override files like `appsettings.Development.json` (gitignored).
- If a secret is ever committed, assume it is compromised and rotate it immediately.

