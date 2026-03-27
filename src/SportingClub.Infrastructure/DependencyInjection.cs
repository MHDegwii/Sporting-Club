using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using SportingClub.Application;

namespace SportingClub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var useLocalDb = configuration.GetValue<bool>("UseLocalDb", false);

        services.AddDbContext<SportingClubDbContext>(options =>
        {
            if (useLocalDb)
            {
                var dbPath = Path.Combine(AppContext.BaseDirectory, "sportingclub.db");
                options.UseSqlite($"Data Source={dbPath}");
            }
            else
            {
                var connectionString = GetConnectionString(configuration);
                options.UseNpgsql(connectionString);
            }
        });

        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddSingleton<INotificationService, FcmNotificationService>();
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        services.AddSingleton<IAnalyticsService, InMemoryAnalyticsService>();
        services.AddScoped<IAuthService, EfCoreAuthService>();
        services.AddScoped<IResourceService, EfCoreResourceService>();
        services.AddHostedService<RenewalReminderBackgroundService>();
        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        var fromConfig = configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(fromConfig))
        {
            return fromConfig;
        }

        // Render typically provides the database via an environment variable.
        var databaseUrl = configuration["DATABASE_URL"];
        if (string.IsNullOrWhiteSpace(databaseUrl))
        {
            // Development fallback. For Render, you should provide DATABASE_URL.
            return "Host=localhost;Port=5432;Database=sportingclub;Username=postgres;Password=postgres";
        }

        // Handle postgres://user:pass@host:port/db style URLs.
        if (databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
        {
            return new NpgsqlConnectionStringBuilder(databaseUrl).ToString();
        }

        return databaseUrl;
    }
}
