@echo off
echo Starting Sporting Club API...
echo.
echo The API will run on: http://localhost:5244
echo.
echo Available Endpoints:
echo - Health: http://localhost:5244/api/v1/health
echo - Swagger UI: http://localhost:5244/swagger
echo.
echo Starting in 3 seconds...
timeout /t 3 /nobreak

cd /d D:\Work\AI-Projects\Sporting-Club
dotnet run --project src/SportingClub.API --no-build
