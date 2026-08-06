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
        // Registered before its consumers for readability only — DI resolves by type.
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserGroupService, UserGroupService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IFunctionGroupService, FunctionGroupService>();
        services.AddScoped<IFunctionService, FunctionService>();

        // MasterData
        services.AddScoped<IFactoryService, FactoryService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<ILineService, LineService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IShiftBreakService, ShiftBreakService>();

        // Equipment
        services.AddScoped<IMachineTypeService, MachineTypeService>();
        services.AddScoped<IMachineService, MachineService>();

        // Add other module services here as the project grows

        return services;
    }
}
