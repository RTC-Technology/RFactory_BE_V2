using Microsoft.Extensions.DependencyInjection;
using RFactory.Application.Modules.Administration.Services;
using RFactory.Application.Modules.Auth.Services;
using RFactory.Application.Modules.Equipment.Services;
using RFactory.Application.Modules.GoodsIssue.Services;
using RFactory.Application.Modules.GoodsReceipt.Services;
using RFactory.Application.Modules.Inventory.Services;
using RFactory.Application.Modules.MasterData.Services;
using RFactory.Application.Modules.Product.Services;
using RFactory.Application.Modules.PurchaseOrder.Services;
using RFactory.Application.Modules.Warehouses.Services;

namespace RFactory.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper: scan this assembly for all Profile classes automatically
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        // Backs the per-user permission cache below.
        services.AddMemoryCache();

        // Auth
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Administration
        // Registered before its consumers for readability only — DI resolves by type.
        // Singleton: the whole point is one flush signal shared by every request.
        services.AddSingleton<IPermissionCacheSignal, PermissionCacheSignal>();
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
        services.AddScoped<IUnitCategoryService, UnitCategoryService>();
        services.AddScoped<IUnitService, UnitService>();
        services.AddScoped<IUnitConversionService, UnitConversionService>();

        // Product
        services.AddScoped<IProductTypeService, ProductTypeService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBomService, BomService>();
        services.AddScoped<IBomDetailService, BomDetailService>();
        services.AddScoped<IRoutingService, RoutingService>();
        services.AddScoped<IRoutingOperationService, RoutingOperationService>();


        // Equipment
        services.AddScoped<IMachineTypeService, MachineTypeService>();
        services.AddScoped<IMachineService, MachineService>();

        //Warehouse
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IWarehouseLocationService, WarehouseLocationService>();
        services.AddScoped<IWarehouseZoneService, WarehouseZoneService>();

        // Add other module services here as the project grows

        //GoodsReceipt
        services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
        services.AddScoped<IGoodsReceiptDetailService, GoodsReceiptDetailService>();

        //Warehouse
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IWarehouseLocationService, WarehouseLocationService>();

        //Supplier
        services.AddScoped<ISupplierService, SupplierService>();

        //GoodsIssue
        services.AddScoped<IGoodsIssueService, GoodsIssueService>();   
        services.AddScoped<IGoodsIssueDetailService, GoodsIssueDetailService>();

        //Inventory
        services.AddScoped<IInventoryService, InventoryService>();   
        services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();   

        //Purchase Order
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();   
        //services.AddScoped<IInventoryTransactionService, InventoryTransactionService>();   


        return services;
    }
}
