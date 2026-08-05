using Microsoft.Extensions.DependencyInjection;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Application.Modules.Auth.Services;
using RFactory.Application.Modules.Equipment.Services;
using RFactory.Application.Modules.MasterData.Services;

namespace RFactory.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper: scan this assembly for all Profile classes automatically
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // Auth
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Administration
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IFunctionGroupService, FunctionGroupService>();
        services.AddScoped<IFunctionService, FunctionService>();

        // MasterData
        services.AddScoped<IFactoryService, FactoryService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<ILineService, LineService>();
        services.AddScoped<IOrganizationService, OrganizationService>();

        // Equipment
        services.AddScoped<IMachineTypeService, MachineTypeService>();
        services.AddScoped<IMachineService, MachineService>();

        // Add other module services here as the project grows

        return services;
    }
}
