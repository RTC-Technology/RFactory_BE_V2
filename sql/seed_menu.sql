-- ============================================================
-- Seed menu data for RFactory MES
-- Chạy script này trong database rtc_factory (MySQL 8.x)
-- ============================================================

-- Xóa dữ liệu menu cũ (nếu muốn seed lại từ đầu)
-- DELETE FROM menu;

-- ============================================================
-- Insert menu data
-- Lưu ý: `Order` phải escape bằng backtick vì là reserved word
-- ============================================================
INSERT INTO menu (Id, CreatedDate, CreatedBy, IsDeleted, Name, Url, Icon, `Order`, ParentId, FunctionId)
VALUES
    -- Root level menus
    (1, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Trang chủ',        '/',           'dashboard',  1, NULL, NULL),
    (2, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Quản lý',          NULL,           'management', 2, NULL, NULL),
    (3, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Sản phẩm',         NULL,           'products',   3, NULL, NULL),
    (4, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Thiết bị',         NULL,           'data',       4, NULL, NULL),
    (5, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Báo cáo',          NULL,           'reports',    5, NULL, NULL),
    (6, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Cài đặt',          NULL,           'settings',   6, NULL, NULL),

    -- Quản lý submenus (ParentId = 2)
    (101, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Người dùng',          '/administration/users',          'users',      1, 2, NULL),
    (102, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Nhóm chức năng',      '/administration/function-groups', 'management', 2, 2, NULL),
    (103, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Chức năng',           '/administration/functions',      'management', 3, 2, NULL),
    (104, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Menu',                '/administration/menus',          'settings',   4, 2, NULL),

    -- Sản phẩm submenus (ParentId = 3)
    (301, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Danh sách sản phẩm',  '/products',          'products', 1, 3, NULL),
    (302, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Loại sản phẩm',       '/products/types',    'products', 2, 3, NULL),
    (303, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Đơn vị tính',         '/products/units',    'data',     3, 3, NULL),
    (304, '2026-01-01 00:00:00', 'SYSTEM', 0, 'BOM',                 '/products/bom',      'data',     4, 3, NULL),

    -- Thiết bị submenus (ParentId = 4)
    (401, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Danh sách máy',       '/equipment/machines',      'data', 1, 4, NULL),
    (402, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Loại máy',            '/equipment/machine-types', 'data', 2, 4, NULL),

    -- Báo cáo submenus (ParentId = 5)
    (501, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Tổng quan',           '/reports/overview',   'analytics', 1, 5, NULL),
    (502, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Sản xuất',            '/reports/production', 'reports',   2, 5, NULL),
    (503, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Thiết bị',            '/reports/equipment',  'reports',   3, 5, NULL),

    -- Cài đặt submenus (ParentId = 6)
    (601, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Công ty',             '/settings/organizations', 'management', 1, 6, NULL),
    (602, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Nhà máy',             '/settings/factories',     'workspace',  2, 6, NULL),
    (603, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Khu vực',             '/settings/areas',         'workspace',  3, 6, NULL),
    (604, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Dây chuyền',          '/settings/lines',         'workspace',  4, 6, NULL),
    (605, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Ca làm việc',         '/settings/shifts',        'calendar',   5, 6, NULL),
    (606, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Ngày nghỉ lễ',        '/settings/holidays',      'calendar',   6, 6, NULL)
ON DUPLICATE KEY UPDATE
    Name = VALUES(Name),
    Url = VALUES(Url),
    Icon = VALUES(Icon),
    `Order` = VALUES(`Order`),
    ParentId = VALUES(ParentId),
    FunctionId = VALUES(FunctionId),
    IsDeleted = 0;

-- ============================================================
-- Verify
-- ============================================================
SELECT
    m.Id,
    m.Name,
    m.Url,
    m.Icon,
    m.`Order`,
    m.ParentId,
    p.Name AS ParentName
FROM menu m
LEFT JOIN menu p ON m.ParentId = p.Id
WHERE m.IsDeleted = 0
ORDER BY COALESCE(m.ParentId, m.Id), m.`Order`;
