-- ============================================================
-- Migration: Add production_order, production_order_material,
-- and production_order_operation tables for RFactory MES
-- Reference: AMIS Quy trình Lập Lệnh sản xuất
-- ============================================================

CREATE TABLE IF NOT EXISTS `production_order` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo',
    `CreatedBy` VARCHAR(50) NOT NULL COMMENT 'Người tạo',
    `UpdatedDate` DATETIME NULL COMMENT 'Thời điểm cập nhật',
    `UpdatedBy` VARCHAR(50) NULL COMMENT 'Người cập nhật',
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Đánh dấu đã xóa',

    `OrderNo` VARCHAR(50) NOT NULL COMMENT 'Mã lệnh sản xuất (duy nhất)',
    `OrderType` INT NOT NULL DEFAULT 4 COMMENT 'Loại lệnh: 1=MTS (Kế hoạch tổng thể), 2=MTO (Đơn đặt hàng), 3=MRP (Kế hoạch chi tiết), 4=Other/Direct (Khác / Hàng mẫu / Đột xuất)',
    `Description` VARCHAR(255) NULL COMMENT 'Mô tả / Tên lệnh sản xuất',
    `ProductId` BIGINT UNSIGNED NOT NULL COMMENT 'ID sản phẩm thành phẩm / bán thành phẩm cần sản xuất',
    `UnitId` BIGINT UNSIGNED NULL COMMENT 'ID đơn vị tính',
    `BomId` BIGINT UNSIGNED NULL COMMENT 'ID định mức BOM áp dụng',
    `RoutingId` BIGINT UNSIGNED NULL COMMENT 'ID quy trình công nghệ Routing áp dụng',
    `FactoryId` BIGINT UNSIGNED NULL COMMENT 'ID nhà máy',
    `LineId` BIGINT UNSIGNED NULL COMMENT 'ID dây chuyền sản xuất',
    `WorkCenterId` BIGINT UNSIGNED NULL COMMENT 'ID tổ / trung tâm gia công / phân xưởng',
    `CustomerId` BIGINT UNSIGNED NULL COMMENT 'ID khách hàng (nếu sản xuất theo đơn khách)',

    `PlannedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Số lượng kế hoạch sản xuất',
    `CompletedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Số lượng đã hoàn thành đạt chuẩn',
    `ScrapQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Số lượng phế phẩm / hỏng',

    `PlannedStartDate` DATETIME NULL COMMENT 'Thời gian bắt đầu kế hoạch',
    `PlannedEndDate` DATETIME NULL COMMENT 'Thời gian kết thúc kế hoạch (Hạn sản xuất)',
    `ActualStartDate` DATETIME NULL COMMENT 'Thời gian bắt đầu thực tế',
    `ActualEndDate` DATETIME NULL COMMENT 'Thời gian kết thúc thực tế',

    `Priority` INT NOT NULL DEFAULT 2 COMMENT 'Độ ưu tiên: 1=Thấp (Low), 2=Bình thường (Normal), 3=Cao (High), 4=Khẩn cấp (Urgent)',
    `Status` INT NOT NULL DEFAULT 1 COMMENT 'Trạng thái: 1=Chưa sản xuất/Dự thảo (Draft), 2=Đã duyệt (Released), 3=Đang sản xuất (InProgress), 4=Đã hoàn thành (Completed), 5=Đã hủy (Cancelled), 6=Đã đóng (Closed), 7=Tạm dừng (Paused)',

    `BatchNo` VARCHAR(100) NULL COMMENT 'Số lô thành phẩm (Lot / Batch No)',
    `SalesOrderNo` VARCHAR(100) NULL COMMENT 'Mã đơn bán hàng tham chiếu (Sales Order / SO)',
    `Remark` TEXT NULL COMMENT 'Ghi chú bổ sung',

    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_production_order_orderno` (`OrderNo`),
    KEY `IX_production_order_product_id` (`ProductId`),
    KEY `IX_production_order_line_id` (`LineId`),
    KEY `IX_production_order_status` (`Status`),
    KEY `IX_production_order_is_deleted` (`IsDeleted`),
    KEY `IX_production_order_created_date` (`CreatedDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Lệnh sản xuất (Production Order / Work Order)';

-- ── Chi tiết nguyên vật liệu (BOM Materials) ─────────────────
CREATE TABLE IF NOT EXISTS `production_order_material` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo',
    `CreatedBy` VARCHAR(50) NOT NULL COMMENT 'Người tạo',
    `UpdatedDate` DATETIME NULL COMMENT 'Thời điểm cập nhật',
    `UpdatedBy` VARCHAR(50) NULL COMMENT 'Người cập nhật',
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Đánh dấu đã xóa',

    `ProductionOrderId` BIGINT UNSIGNED NOT NULL COMMENT 'ID lệnh sản xuất',
    `ProductId` BIGINT UNSIGNED NOT NULL COMMENT 'ID nguyên vật liệu',
    `ProductCode` VARCHAR(50) NULL COMMENT 'Mã NVL',
    `ProductName` VARCHAR(255) NULL COMMENT 'Tên NVL',
    `UnitId` BIGINT UNSIGNED NULL COMMENT 'ID đơn vị tính',
    `UnitName` VARCHAR(50) NULL COMMENT 'Đơn vị tính',

    `NormQuantity` DECIMAL(18, 6) NOT NULL DEFAULT 0.000000 COMMENT 'Định mức theo 1 đơn vị thành phẩm',
    `RequiredQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'SL theo định mức (= NormQuantity * PlannedQuantity)',
    `IssuedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'SL thực tế đã xuất kho',
    `CurrentStock` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Tồn kho khả dụng hiện tại',
    `WarehouseId` BIGINT UNSIGNED NULL COMMENT 'Kho xuất vật tư',
    `WarehouseName` VARCHAR(100) NULL COMMENT 'Tên kho xuất',

    `IsSubstituted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Đánh dấu vật tư thay thế',
    `OriginalProductId` BIGINT UNSIGNED NULL COMMENT 'ID NVL ban đầu nếu là vật tư thay thế',
    `Remark` VARCHAR(255) NULL COMMENT 'Ghi chú',

    PRIMARY KEY (`Id`),
    KEY `IX_prod_order_mat_order_id` (`ProductionOrderId`),
    KEY `IX_prod_order_mat_product_id` (`ProductId`),
    KEY `IX_prod_order_mat_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Chi tiết nguyên vật liệu của Lệnh sản xuất';

-- ── Công đoạn sản xuất (Routing Operations) ──────────────────
CREATE TABLE IF NOT EXISTS `production_order_operation` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Primary Key',
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo',
    `CreatedBy` VARCHAR(50) NOT NULL COMMENT 'Người tạo',
    `UpdatedDate` DATETIME NULL COMMENT 'Thời điểm cập nhật',
    `UpdatedBy` VARCHAR(50) NULL COMMENT 'Người cập nhật',
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Đánh dấu đã xóa',

    `ProductionOrderId` BIGINT UNSIGNED NOT NULL COMMENT 'ID lệnh sản xuất',
    `RoutingOperationId` BIGINT UNSIGNED NULL COMMENT 'ID công đoạn gốc trong Routing',
    `Sequence` INT NOT NULL DEFAULT 1 COMMENT 'STT / Thứ tự công đoạn',
    `OperationCode` VARCHAR(50) NOT NULL COMMENT 'Mã công đoạn',
    `OperationName` VARCHAR(255) NOT NULL COMMENT 'Tên công đoạn',

    `WorkCenterId` BIGINT UNSIGNED NULL COMMENT 'ID tổ / phân xưởng / trung tâm gia công',
    `WorkCenterName` VARCHAR(100) NULL COMMENT 'Tên tổ / phân xưởng',
    `MachineId` BIGINT UNSIGNED NULL COMMENT 'ID máy gán cho công đoạn',
    `MachineName` VARCHAR(100) NULL COMMENT 'Tên / Mã máy',
    `AssignedEmployeeId` BIGINT UNSIGNED NULL COMMENT 'ID người / thợ phụ trách thực hiện',
    `AssignedEmployeeName` VARCHAR(100) NULL COMMENT 'Tên người thực hiện',

    `PlannedStartDate` DATETIME NULL COMMENT 'Thời gian bắt đầu công đoạn kế hoạch',
    `PlannedEndDate` DATETIME NULL COMMENT 'Thời gian kết thúc công đoạn kế hoạch',
    `ActualStartDate` DATETIME NULL COMMENT 'Thời gian bắt đầu thực tế',
    `ActualEndDate` DATETIME NULL COMMENT 'Thời gian kết thúc thực tế',

    `Status` INT NOT NULL DEFAULT 1 COMMENT 'Trạng thái: 1=Chưa thực hiện, 2=Đang thực hiện, 3=Hoàn thành, 4=Bỏ qua',
    `Remark` VARCHAR(255) NULL COMMENT 'Ghi chú công đoạn',

    PRIMARY KEY (`Id`),
    KEY `IX_prod_order_op_order_id` (`ProductionOrderId`),
    KEY `IX_prod_order_op_seq` (`Sequence`),
    KEY `IX_prod_order_op_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Danh sách công đoạn sản xuất của Lệnh sản xuất';
