# Deploy Sporting Club API on Koyeb + Neon

This setup avoids card verification on Render and keeps data persistent with PostgreSQL.

## 1) Create Neon PostgreSQL

1. Sign in to Neon and create a new project.
2. Copy the connection string from Neon (pooled/direct URI both work).
3. Keep it for Koyeb as `DATABASE_URL`.

## 2) Create Koyeb Web Service from GitHub

1. In Koyeb, create a new **Web Service** from your GitHub repo.
2. Select repository: `MHDegwii/Sporting-Club`
3. Select branch: `dev`
4. Build method: **Dockerfile**
5. Dockerfile path: `./Dockerfile`
6. Exposed port: `8080`

## 3) Set Environment Variables in Koyeb

Add these variables in service settings:

- `DATABASE_URL` = `<Neon connection string>`
- `Jwt__Key` = `<long random secret>`
- `Jwt__Issuer` = `SportingClub`
- `Jwt__Audience` = `SportingClubClients`
- `Jwt__AccessTokenExpiryMinutes` = `60`
- `ASPNETCORE_ENVIRONMENT` = `Production`

## 4) Deploy

1. Trigger deployment.
2. Wait until Koyeb shows service as **Healthy**.
3. Open:
   - API root: `https://<your-koyeb-domain>`
   - Swagger: `https://<your-koyeb-domain>/swagger`
   - Health: `https://<your-koyeb-domain>/api/v1/health`

## Notes

- The app auto-runs EF migrations on startup (`Database.Migrate()`), so tables are created in Neon on first boot.
- File uploads currently write to local container disk. For durable file storage, move to S3/R2 later.
