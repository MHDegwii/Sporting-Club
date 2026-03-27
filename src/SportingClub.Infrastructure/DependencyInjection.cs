using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportingClub.Application;

namespace SportingClub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddSingleton<INotificationService, FcmNotificationService>();
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();
        services.AddSingleton<IAnalyticsService, InMemoryAnalyticsService>();
        services.AddSingleton<IAuthService, InMemoryAuthService>();
        services.AddSingleton<IResourceService, InMemoryResourceService>();
        services.AddHostedService<RenewalReminderBackgroundService>();
        return services;
    }
}
