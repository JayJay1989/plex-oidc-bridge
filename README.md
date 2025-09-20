# Plex OIDC Bridge for Keycloak

A lightweight **.NET 8** OIDC Provider that lets you use **Plex login** as an Identity Provider inside **Keycloak** or any OpenID Connect client.

?? Uses Plex’s official **PIN-based authentication flow**  
?? Exposes standard OIDC endpoints (`/.well-known/openid-configuration`, `/authorize`, `/token`, `/jwks`, `/userinfo`)  
?? Includes an **Admin Panel** to manage OIDC clients (CRUD UI with session-based login `/admin`)  

---

## ? Features

- Full **OIDC provider** implementation (Authorization Code Flow + PKCE).  
- **Plex PIN login** flow auto-opens Plex’s site, polls until approved, and returns to Keycloak.  
- **SQLite + EF Core** persistence (sessions, codes, keys, clients, admin users).  
- **RSA key** stored in DB, exposed via JWKS for token validation.  
- **Admin Panel** (Razor Pages) with CRUD for clients:  
  - Manage `clientId`, `clientSecret`, and `redirectUri`.  
  - Seeded **admin user** (credentials from `appsettings.json`).  
  - Logs a **warning** if the admin password is left as the default `"admin"`.  
- **Server-Sent Events (SSE)** used for Plex PIN polling (instead of browser polling).

---

## ?? Getting Started

### Prerequisites
- .NET 8 SDK  
- SQLite (file is auto-created)  
- Keycloak 26.x+ (or another OIDC client)

### Run

```bash
dotnet run
````


#### Docker

example `docker-compose.yml`

```bash
services:
  minecraft-notifier:
    container_name: plex-sso
    image: ghcr.io/jayjay1989/plexsso:latest
    restart: unless-stopped
    ports:
      - 8080:8080
    environment:
      - TZ=Europe/Brussels
      - ConnectionStrings__DefaultConnection=Data Source=/app/db/plex-oidc-bridge.db;
      - Admin__Username=admin
      - Admin__Password=admin
      - SSO__Issuer=https://your-plex-sso-url
      - Plex__ClientIdentifier=your-identifier
    volumes:
      - db-data:/app/db

volumes:
  db-data:
```
