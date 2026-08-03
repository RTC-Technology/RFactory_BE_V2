using Microsoft.Extensions.DependencyInjection;
using RFactory.BackgroundService.Workers;

namespace RFactory.BackgroundService.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<SampleWorker>();
        return services;
    }
}
