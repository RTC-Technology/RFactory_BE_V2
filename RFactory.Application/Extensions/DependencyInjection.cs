using Microsoft.Extensions.DependencyInjection;
using RFactory.Application.Modules.MasterData.Services;

namespace RFactory.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper: scan this assembly for all Profile classes automatically
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // MasterData
        services.AddScoped<IFactoryService, FactoryService>();

        // Add other module services here as the project grows

        return services;
    }
}
