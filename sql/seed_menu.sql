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
    -- ── Root level menus ──────────────────────────────────────
    (1, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Trang chủ',             '/',           'dashboard',  1, NULL, NULL),
    (2, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Quản lý hệ thống',      NULL,          'management', 2, NULL, NULL),
    (3, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Sản phẩm',             NULL,          'products',   3, NULL, NULL),
    (4, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Thiết bị & OEE',        NULL,          'data',       4, NULL, NULL),
    (5, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Đóng gói & Nhãn QR',    NULL,          'box',        5, NULL, NULL),
    (6, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Kho & Logistics',       NULL,          'warehouse',  6, NULL, NULL),
    (7, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Mua hàng & NCC',        NULL,          'cart',       7, NULL, NULL),
    (8, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Nhân sự & Kỹ năng',     NULL,          'users',      8, NULL, NULL),
    (9, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Cài đặt nhà máy',       NULL,          'settings',   9, NULL, NULL),

    -- ── Quản lý hệ thống (ParentId = 2) ──────────────────────
    (101, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Người dùng & Tổ chức', '/users',          'users',      1, 2, NULL),
    (102, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Nhóm quyền',          '/user-groups',   'management', 2, 2, NULL),
    (103, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Chức năng',           '/functions',     'management', 3, 2, NULL),
    (104, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Cấu hình Menu',       '/menu-manager',  'settings',   4, 2, NULL),

    -- ── Sản phẩm (ParentId = 3) ──────────────────────────────
    (301, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Danh sách sản phẩm',  '/products',          'products',   1, 3, NULL),
    (302, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Loại sản phẩm',       '/product-types',     'products',   2, 3, NULL),
    (303, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Đơn vị tính',         '/units',             'data',       3, 3, NULL),
    (304, '2026-01-01 00:00:00', 'SYSTEM', 0, 'BOM',                 '/products/bom',      'box',        4, 3, NULL),
    (305, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Lệnh sản xuất',       '/production-orders', 'workspace',  5, 3, NULL),

    -- ── Thiết bị & OEE (ParentId = 4) ────────────────────────
    (401, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Danh sách máy',       '/machines',       'data',      1, 4, NULL),
    (402, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Trạng thái & Sự cố',  '/machine-status', 'analytics', 2, 4, NULL),

    -- ── Đóng gói & Nhãn QR (ParentId = 5) ────────────────────
    (501, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Quy cách đóng gói',   '/packing-specifications',    'box', 1, 5, NULL),
    (502, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Quy tắc đánh số',     '/packing-numbering-rules',   'tag', 2, 5, NULL),
    (503, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Tạo mã QR & Nhãn',    '/qr-code',                  'qr',  3, 5, NULL),
    (504, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Mẫu in nhãn',         '/label-templates',          'tag', 4, 5, NULL),

    -- ── Kho & Logistics (ParentId = 6) ───────────────────────
    (601, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Kho & Vị trí',        '/warehouses',    'warehouse',  1, 6, NULL),
    (602, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Tồn kho',             '/inventory',     'data',       2, 6, NULL),
    (603, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Phiếu nhập kho',      '/goods-receipt', 'inbox',      3, 6, NULL),
    (604, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Phiếu xuất kho',      '/goods-issue',   'outbox',     4, 6, NULL),

    -- ── Mua hàng & NCC (ParentId = 7) ────────────────────────
    (701, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Đơn mua hàng (PO)',   '/purchase-order','cart',       1, 7, NULL),
    (702, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Nhà cung cấp',        '/supplier',      'briefcase',  2, 7, NULL),

    -- ── Nhân sự & Kỹ năng (ParentId = 8) ─────────────────────
    (801, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Hồ sơ nhân viên',     '/employees',     'users',      1, 8, NULL),
    (802, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Chức vụ',             '/positions',     'badge',      2, 8, NULL),
    (803, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Kỹ năng tay nghề',    '/skills',        'star',       3, 8, NULL),

    -- ── Cài đặt nhà máy (ParentId = 9) ───────────────────────
    (901, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Cài đặt chung',        '/settings',          'settings',  1, 9, NULL),
    (902, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Cơ cấu nhà máy',      '/factory-structure', 'workspace', 2, 9, NULL),
    (903, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Dây chuyền sản xuất', '/lines',             'workspace', 3, 9, NULL),
    (904, '2026-01-01 00:00:00', 'SYSTEM', 0, 'Ca làm việc & Nghỉ',  '/shifts',            'calendar',  4, 9, NULL)
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
