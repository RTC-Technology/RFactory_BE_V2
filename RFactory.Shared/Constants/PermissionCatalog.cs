namespace RFactory.Shared.Constants;

/// <summary>One permission: the code the API checks and the label an admin reads.</summary>
public sealed record PermissionSpec(string Code, string Name);

/// <summary>
/// A permission group, which is an entity. Its permissions are the actions on that entity —
/// the same shape the admin screen shows: pick the group on the left, see its actions on
/// the right.
/// </summary>
public sealed record PermissionGroupSpec(string Code, string Name, IReadOnlyList<PermissionSpec> Permissions);

/// <summary>
/// The full set of groups and permissions the application enforces, ready to be written
/// into the FunctionGroup / Function tables.
///
/// Codes come from <see cref="PermissionCodes"/> rather than being repeated as literals,
/// so the catalogue cannot drift away from what the endpoints actually check.
///
/// Names are Vietnamese because the Function table holds one language, same as Menu.
/// </summary>
public static class PermissionCatalog
{
    public static IReadOnlyList<PermissionGroupSpec> Groups { get; } = new PermissionGroupSpec[]
    {
        new("factory", "Nhà máy", new PermissionSpec[]
        {
            new(PermissionCodes.Factory.View,   "Xem danh sách nhà máy"),
            new(PermissionCodes.Factory.Add,    "Thêm nhà máy"),
            new(PermissionCodes.Factory.Edit,   "Sửa nhà máy"),
            new(PermissionCodes.Factory.Delete, "Xóa nhà máy"),
        }),
        new("area", "Khu vực", new PermissionSpec[]
        {
            new(PermissionCodes.Area.View,   "Xem danh sách khu vực"),
            new(PermissionCodes.Area.Add,    "Thêm khu vực"),
            new(PermissionCodes.Area.Edit,   "Sửa khu vực"),
            new(PermissionCodes.Area.Delete, "Xóa khu vực"),
        }),
        new("line", "Line", new PermissionSpec[]
        {
            new(PermissionCodes.Line.View,   "Xem danh sách line"),
            new(PermissionCodes.Line.Add,    "Thêm line"),
            new(PermissionCodes.Line.Edit,   "Sửa line"),
            new(PermissionCodes.Line.Delete, "Xóa line"),
        }),
        new("unit-category", "Nhóm đơn vị tính", new PermissionSpec[]
        {
            new(PermissionCodes.UnitCategory.View,   "Xem danh sách nhóm đơn vị"),
            new(PermissionCodes.UnitCategory.Add,    "Thêm nhóm đơn vị"),
            new(PermissionCodes.UnitCategory.Edit,   "Sửa nhóm đơn vị"),
            new(PermissionCodes.UnitCategory.Delete, "Xóa nhóm đơn vị"),
        }),
        new("unit-conversion", "Quy đổi đơn vị", new PermissionSpec[]
        {
            new(PermissionCodes.UnitConversion.View,   "Xem quy đổi đơn vị"),
            new(PermissionCodes.UnitConversion.Add,    "Thêm quy đổi"),
            new(PermissionCodes.UnitConversion.Edit,   "Sửa quy đổi"),
            new(PermissionCodes.UnitConversion.Delete, "Xóa quy đổi"),
        }),
        new("unit", "Đơn vị tính", new PermissionSpec[]
        {
            new(PermissionCodes.Unit.View,   "Xem danh sách đơn vị tính"),
            new(PermissionCodes.Unit.Add,    "Thêm đơn vị tính"),
            new(PermissionCodes.Unit.Edit,   "Sửa đơn vị tính"),
            new(PermissionCodes.Unit.Delete, "Xóa đơn vị tính"),
        }),
        new("product-type", "Loại sản phẩm", new PermissionSpec[]
        {
            new(PermissionCodes.ProductType.View,   "Xem danh sách loại sản phẩm"),
            new(PermissionCodes.ProductType.Add,    "Thêm loại sản phẩm"),
            new(PermissionCodes.ProductType.Edit,   "Sửa loại sản phẩm"),
            new(PermissionCodes.ProductType.Delete, "Xóa loại sản phẩm"),
        }),
        new("product", "Sản phẩm", new PermissionSpec[]
        {
            new(PermissionCodes.Product.View,   "Xem danh sách sản phẩm"),
            new(PermissionCodes.Product.Add,    "Thêm sản phẩm"),
            new(PermissionCodes.Product.Edit,   "Sửa sản phẩm"),
            new(PermissionCodes.Product.Delete, "Xóa sản phẩm"),
        }),
        new("bom", "Định mức nguyên vật liệu (BOM)", new PermissionSpec[]
        {
            new(PermissionCodes.Bom.View,   "Xem danh sách BOM"),
            new(PermissionCodes.Bom.Add,    "Thêm BOM"),
            new(PermissionCodes.Bom.Edit,   "Sửa BOM"),
            new(PermissionCodes.Bom.Delete, "Xóa BOM"),
        }),
        new("bom-detail", "Chi tiết BOM", new PermissionSpec[]
        {
            new(PermissionCodes.BomDetail.View,   "Xem chi tiết BOM"),
            new(PermissionCodes.BomDetail.Add,    "Thêm dòng BOM"),
            new(PermissionCodes.BomDetail.Edit,   "Sửa dòng BOM"),
            new(PermissionCodes.BomDetail.Delete, "Xóa dòng BOM"),
        }),
        new("shift", "Ca làm việc", new PermissionSpec[]
        {
            new(PermissionCodes.Shift.View,   "Xem danh sách ca làm việc"),
            new(PermissionCodes.Shift.Add,    "Thêm ca làm việc"),
            new(PermissionCodes.Shift.Edit,   "Sửa ca làm việc"),
            new(PermissionCodes.Shift.Delete, "Xóa ca làm việc"),
        }),
        new("shift-break", "Giờ nghỉ trong ca", new PermissionSpec[]
        {
            new(PermissionCodes.ShiftBreak.View,   "Xem giờ nghỉ trong ca"),
            new(PermissionCodes.ShiftBreak.Add,    "Thêm giờ nghỉ"),
            new(PermissionCodes.ShiftBreak.Edit,   "Sửa giờ nghỉ"),
            new(PermissionCodes.ShiftBreak.Delete, "Xóa giờ nghỉ"),
        }),
        new("machine-type", "Loại máy", new PermissionSpec[]
        {
            new(PermissionCodes.MachineType.View,   "Xem danh sách loại máy"),
            new(PermissionCodes.MachineType.Add,    "Thêm loại máy"),
            new(PermissionCodes.MachineType.Edit,   "Sửa loại máy"),
            new(PermissionCodes.MachineType.Delete, "Xóa loại máy"),
        }),
        new("machine", "Máy", new PermissionSpec[]
        {
            new(PermissionCodes.Machine.View,   "Xem danh sách máy"),
            new(PermissionCodes.Machine.Add,    "Thêm máy"),
            new(PermissionCodes.Machine.Edit,   "Sửa máy"),
            new(PermissionCodes.Machine.Delete, "Xóa máy"),
        }),
        new("organization", "Cơ cấu tổ chức", new PermissionSpec[]
        {
            new(PermissionCodes.Organization.View,   "Xem cơ cấu tổ chức"),
            new(PermissionCodes.Organization.Add,    "Thêm tổ chức"),
            new(PermissionCodes.Organization.Edit,   "Sửa tổ chức"),
            new(PermissionCodes.Organization.Delete, "Xóa tổ chức"),
        }),
        new("user", "Người dùng", new PermissionSpec[]
        {
            new(PermissionCodes.User.View,   "Xem danh sách người dùng"),
            new(PermissionCodes.User.Add,    "Thêm người dùng"),
            new(PermissionCodes.User.Edit,   "Sửa người dùng"),
            new(PermissionCodes.User.Delete, "Xóa người dùng"),
        }),
        new("user-group", "Nhóm người dùng", new PermissionSpec[]
        {
            new(PermissionCodes.UserGroup.View,   "Xem danh sách nhóm người dùng"),
            new(PermissionCodes.UserGroup.Add,    "Thêm nhóm người dùng"),
            new(PermissionCodes.UserGroup.Edit,   "Sửa nhóm, phân quyền và gán nhân viên"),
            new(PermissionCodes.UserGroup.Delete, "Xóa nhóm người dùng"),
        }),
        new("function-group", "Nhóm quyền", new PermissionSpec[]
        {
            new(PermissionCodes.FunctionGroup.View,   "Xem danh sách nhóm quyền"),
            new(PermissionCodes.FunctionGroup.Add,    "Thêm nhóm quyền"),
            new(PermissionCodes.FunctionGroup.Edit,   "Sửa nhóm quyền"),
            new(PermissionCodes.FunctionGroup.Delete, "Xóa nhóm quyền"),
        }),
        new("function", "Quyền", new PermissionSpec[]
        {
            new(PermissionCodes.Function.View,   "Xem danh sách quyền"),
            new(PermissionCodes.Function.Add,    "Thêm quyền"),
            new(PermissionCodes.Function.Edit,   "Sửa quyền"),
            new(PermissionCodes.Function.Delete, "Xóa quyền"),
        }),
        new("menu", "Menu", new PermissionSpec[]
        {
            new(PermissionCodes.Menu.View,   "Xem khai báo menu"),
            new(PermissionCodes.Menu.Add,    "Thêm menu"),
            new(PermissionCodes.Menu.Edit,   "Sửa menu"),
            new(PermissionCodes.Menu.Delete, "Xóa menu"),
        }),
        new("goods-receipt", "Phiếu nhập kho", new PermissionSpec[]
        {
            new(PermissionCodes.GoodsReceipt.View,   "Xem danh sách phiếu nhập"),
            new(PermissionCodes.GoodsReceipt.Add,    "Thêm phiếu nhập"),
            new(PermissionCodes.GoodsReceipt.Edit,   "Sửa phiếu nhập"),
            new(PermissionCodes.GoodsReceipt.Delete, "Xóa phiếu nhập"),
        }),
        new("goods-receipt-detail", "Chi tiết phiếu nhập", new PermissionSpec[]
        {
            new(PermissionCodes.GoodsReceiptDetail.View,   "Xem chi tiết phiếu nhập"),
            new(PermissionCodes.GoodsReceiptDetail.Add,    "Thêm dòng phiếu nhập"),
            new(PermissionCodes.GoodsReceiptDetail.Edit,   "Sửa dòng phiếu nhập"),
            new(PermissionCodes.GoodsReceiptDetail.Delete, "Xóa dòng phiếu nhập"),
        }),
        new("settings", "Cài đặt", new PermissionSpec[]
        {
            new(PermissionCodes.Settings.View, "Xem cài đặt hệ thống"),
            new(PermissionCodes.Settings.Edit, "Sửa cài đặt hệ thống"),
        }),
    };

    /// <summary>Every code in the catalogue, flattened.</summary>
    public static IReadOnlyList<string> AllCodes { get; } =
        Groups.SelectMany(g => g.Permissions).Select(p => p.Code).ToList();
}
