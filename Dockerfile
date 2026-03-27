FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["SportingClub.sln", "./"]
COPY ["src/SportingClub.Domain/SportingClub.Domain.csproj", "src/SportingClub.Domain/"]
COPY ["src/SportingClub.Application/SportingClub.Application.csproj", "src/SportingClub.Application/"]
COPY ["src/SportingClub.Infrastructure/SportingClub.Infrastructure.csproj", "src/SportingClub.Infrastructure/"]
COPY ["src/SportingClub.API/SportingClub.API.csproj", "src/SportingClub.API/"]

RUN dotnet restore "SportingClub.sln"

COPY . .
RUN dotnet publish "src/SportingClub.API/SportingClub.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SportingClub.API.dll"]
