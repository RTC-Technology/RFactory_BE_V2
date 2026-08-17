namespace RFactory.Shared.Constants;

/// <summary>
/// Every permission code the API enforces, as <c>&lt;entity&gt;.&lt;action&gt;</c>.
///
/// The four actions line up with the HTTP verbs a controller exposes — <c>view</c> for
/// GET, <c>add</c> for POST, <c>edit</c> for PUT, <c>delete</c> for DELETE — so gating an
/// endpoint is mechanical and nobody has to invent a code.
///
/// Splitting <c>view</c> out from the write actions is what makes gating reads workable:
/// the machine screen also loads lines and areas, so its operator needs
/// <c>line.view</c> and <c>area.view</c> without ever getting <c>line.edit</c>.
///
/// Nested classes so call sites read as <c>PermissionCodes.Factory.Add</c>. Mirrors
/// <c>PERMISSIONS</c> in the Angular client — the two gate the same features from either
/// end and have to agree.
/// </summary>
public static class PermissionCodes
{
    public const string View = "view";
    public const string Add = "add";
    public const string Edit = "edit";
    public const string Delete = "delete";

    public static class Factory
    {
        public const string View = "factory.view";
        public const string Add = "factory.add";
        public const string Edit = "factory.edit";
        public const string Delete = "factory.delete";
    }

    public static class Area
    {
        public const string View = "area.view";
        public const string Add = "area.add";
        public const string Edit = "area.edit";
        public const string Delete = "area.delete";
    }

    public static class Line
    {
        public const string View = "line.view";
        public const string Add = "line.add";
        public const string Edit = "line.edit";
        public const string Delete = "line.delete";
    }

    public static class UnitCategory
    {
        public const string View = "unit-category.view";
        public const string Add = "unit-category.add";
        public const string Edit = "unit-category.edit";
        public const string Delete = "unit-category.delete";
    }

    public static class UnitConversion
    {
        public const string View = "unit-conversion.view";
        public const string Add = "unit-conversion.add";
        public const string Edit = "unit-conversion.edit";
        public const string Delete = "unit-conversion.delete";
    }

    public static class Unit
    {
        public const string View = "unit.view";
        public const string Add = "unit.add";
        public const string Edit = "unit.edit";
        public const string Delete = "unit.delete";
    }

    public static class ProductType
    {
        public const string View = "product-type.view";
        public const string Add = "product-type.add";
        public const string Edit = "product-type.edit";
        public const string Delete = "product-type.delete";
    }

    public static class Product
    {
        public const string View = "product.view";
        public const string Add = "product.add";
        public const string Edit = "product.edit";
        public const string Delete = "product.delete";
    }

    public static class Bom
    {
        public const string View = "bom.view";
        public const string Add = "bom.add";
        public const string Edit = "bom.edit";
        public const string Delete = "bom.delete";
    }

    public static class BomDetail
    {
        public const string View = "bom-detail.view";
        public const string Add = "bom-detail.add";
        public const string Edit = "bom-detail.edit";
        public const string Delete = "bom-detail.delete";
    }

    public static class Routing
    {
        public const string View = "routing.view";
        public const string Add = "routing.add";
        public const string Edit = "routing.edit";
        public const string Delete = "routing.delete";
    }

    public static class RoutingOperation
    {
        public const string View = "routing-operation.view";
        public const string Add = "routing-operation.add";
        public const string Edit = "routing-operation.edit";
        public const string Delete = "routing-operation.delete";
    }

    public static class Shift
    {
        public const string View = "shift.view";
        public const string Add = "shift.add";
        public const string Edit = "shift.edit";
        public const string Delete = "shift.delete";
    }

    public static class ShiftBreak
    {
        public const string View = "shift-break.view";
        public const string Add = "shift-break.add";
        public const string Edit = "shift-break.edit";
        public const string Delete = "shift-break.delete";
    }

    public static class MachineType
    {
        public const string View = "machine-type.view";
        public const string Add = "machine-type.add";
        public const string Edit = "machine-type.edit";
        public const string Delete = "machine-type.delete";
    }

    public static class Machine
    {
        public const string View = "machine.view";
        public const string Add = "machine.add";
        public const string Edit = "machine.edit";
        public const string Delete = "machine.delete";
    }

    public static class Organization
    {
        public const string View = "organization.view";
        public const string Add = "organization.add";
        public const string Edit = "organization.edit";
        public const string Delete = "organization.delete";
    }

    public static class User
    {
        public const string View = "user.view";
        public const string Add = "user.add";
        public const string Edit = "user.edit";
        public const string Delete = "user.delete";
    }

    public static class UserGroup
    {
        public const string View = "user-group.view";
        public const string Add = "user-group.add";
        public const string Edit = "user-group.edit";
        public const string Delete = "user-group.delete";
    }

    public static class FunctionGroup
    {
        public const string View = "function-group.view";
        public const string Add = "function-group.add";
        public const string Edit = "function-group.edit";
        public const string Delete = "function-group.delete";
    }

    public static class Function
    {
        public const string View = "function.view";
        public const string Add = "function.add";
        public const string Edit = "function.edit";
        public const string Delete = "function.delete";
    }

    public static class Menu
    {
        public const string View = "menu.view";
        public const string Add = "menu.add";
        public const string Edit = "menu.edit";
        public const string Delete = "menu.delete";
    }

    public static class Warehouse
    {
        public const string View = "warehouse.view";
        public const string Add = "warehouse.add";
        public const string Edit = "warehouse.edit";
        public const string Delete = "warehouse.delete";
    }

    public static class WarehouseZone
    {
        public const string View = "warehouse-zone.view";
        public const string Add = "warehouse-zone.add";
        public const string Edit = "warehouse-zone.edit";
        public const string Delete = "warehouse-zone.delete";
    }

    public static class WarehouseLocation
    {
        public const string View = "warehouse-location.view";
        public const string Add = "warehouse-location.add";
        public const string Edit = "warehouse-location.edit";
        public const string Delete = "warehouse-location.delete";
    }

    public static class GoodsReceipt
    {
        public const string View = "goods-receipt.view";
        public const string Add = "goods-receipt.add";
        public const string Edit = "goods-receipt.edit";
        public const string Delete = "goods-receipt.delete";
    }

    public static class GoodsReceiptDetail
    {
        public const string View = "goods-receipt-detail.view";
        public const string Add = "goods-receipt-detail.add";
        public const string Edit = "goods-receipt-detail.edit";
        public const string Delete = "goods-receipt-detail.delete";
    }

    public static class Supplier
    {
        public const string View = "supplier.view";
        public const string Add = "supplier.add";
        public const string Edit = "supplier.edit";
        public const string Delete = "supplier.delete";
    }

    public static class Settings
    {
        public const string View = "settings.view";
        public const string Edit = "settings.edit";
    }

}
