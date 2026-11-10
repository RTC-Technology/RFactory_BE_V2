-- ============================================================
-- Migration: Add Detailed Production Plan tables for RFactory MES
-- Reference: AMIS Quy trình Lập Kế hoạch chi tiết
-- ============================================================

-- 1. Kế hoạch tổng thể (Master Production Plan)
CREATE TABLE IF NOT EXISTS `master_production_plan` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT 'Primary Key',
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo',
    `CreatedBy` VARCHAR(50) NOT NULL COMMENT 'Người tạo',
    `UpdatedDate` DATETIME NULL COMMENT 'Thời điểm cập nhật',
    `UpdatedBy` VARCHAR(50) NULL COMMENT 'Người cập nhật',
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Đánh dấu đã xóa',

    `PlanCode` VARCHAR(50) NOT NULL COMMENT 'Mã kế hoạch tổng thể (duy nhất)',
    `PlanName` VARCHAR(255) NOT NULL COMMENT 'Tên kế hoạch tổng thể',
    `PeriodType` VARCHAR(20) NOT NULL DEFAULT 'MONTH' COMMENT 'Loại kỳ: MONTH, QUARTER, YEAR',
    `PeriodName` VARCHAR(50) NULL COMMENT 'Tên kỳ: VD Tháng 10/2026',
    `FromDate` DATE NOT NULL COMMENT 'Từ ngày',
    `ToDate` DATE NOT NULL COMMENT 'Đến ngày',
    `WorkshopId` BIGINT UNSIGNED NULL COMMENT 'Phân xưởng / Xưởng',
    `WorkshopName` VARCHAR(100) NULL COMMENT 'Tên xưởng',
    `Status` INT NOT NULL DEFAULT 2 COMMENT '1=Draft, 2=Approved, 3=InProgress, 4=Completed, 5=Cancelled',
    `TotalQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Tổng số lượng sản xuất',
    `Remark` TEXT NULL COMMENT 'Ghi chú',

    UNIQUE KEY `UX_master_prod_plan_code` (`PlanCode`),
    KEY `IX_master_prod_plan_status` (`Status`),
    KEY `IX_master_prod_plan_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Kế hoạch sản xuất tổng thể (Master Production Plan)';

-- 2. Chi tiết sản phẩm kế hoạch tổng thể
CREATE TABLE IF NOT EXISTS `master_production_plan_item` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CreatedBy` VARCHAR(50) NOT NULL,
    `UpdatedDate` DATETIME NULL,
    `UpdatedBy` VARCHAR(50) NULL,
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0,

    `MasterPlanId` BIGINT UNSIGNED NOT NULL,
    `ProductId` BIGINT UNSIGNED NOT NULL,
    `ProductCode` VARCHAR(50) NOT NULL,
    `ProductName` VARCHAR(255) NOT NULL,
    `UnitId` BIGINT UNSIGNED NULL,
    `UnitName` VARCHAR(50) NULL,
    `PlannedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    `AllocatedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    `RemainingQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000,
    `DailyCapacity` DECIMAL(18, 4) NOT NULL DEFAULT 100.0000,
    `Remark` VARCHAR(255) NULL,

    KEY `IX_master_item_plan_id` (`MasterPlanId`),
    KEY `IX_master_item_product_id` (`ProductId`),
    KEY `IX_master_item_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Chi tiết sản phẩm kế hoạch tổng thể';

-- 3. Kế hoạch sản xuất chi tiết (Detailed Production Plan)
CREATE TABLE IF NOT EXISTS `detailed_production_plan` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT 'Primary Key',
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Thời điểm tạo',
    `CreatedBy` VARCHAR(50) NOT NULL COMMENT 'Người tạo',
    `UpdatedDate` DATETIME NULL COMMENT 'Thời điểm cập nhật',
    `UpdatedBy` VARCHAR(50) NULL COMMENT 'Người cập nhật',
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Đánh dấu đã xóa',

    `PlanCode` VARCHAR(50) NOT NULL COMMENT 'Số / Mã kế hoạch chi tiết (duy nhất)',
    `PlanName` VARCHAR(255) NOT NULL COMMENT 'Tên kế hoạch chi tiết',
    `MasterPlanId` BIGINT UNSIGNED NULL COMMENT 'ID kế hoạch tổng thể nguồn',
    `MasterPlanCode` VARCHAR(50) NULL COMMENT 'Số kế hoạch tổng thể nguồn',
    `WorkshopId` BIGINT UNSIGNED NULL COMMENT 'ID đơn vị / phân xưởng',
    `WorkshopName` VARCHAR(100) NULL COMMENT 'Tên đơn vị / phân xưởng',
    `LineId` BIGINT UNSIGNED NULL COMMENT 'ID dây chuyền',
    `LineName` VARCHAR(100) NULL COMMENT 'Tên dây chuyền',
    `DepartmentId` BIGINT UNSIGNED NULL,
    `DepartmentName` VARCHAR(100) NULL,
    `PeriodType` VARCHAR(20) NOT NULL DEFAULT 'DAY' COMMENT 'Kỳ lập chi tiết: DAY (Ngày) hoặc SHIFT (Ca)',
    `FromDate` DATE NOT NULL COMMENT 'Từ ngày',
    `ToDate` DATE NOT NULL COMMENT 'Đến ngày',
    `AllocationMethod` VARCHAR(20) NOT NULL DEFAULT 'FORWARD' COMMENT 'Phương pháp phân bổ: FORWARD (Tiến), BACKWARD (Lùi), MANUAL (Tùy chỉnh)',
    `Status` INT NOT NULL DEFAULT 1 COMMENT 'Trạng thái: 1=Draft (Dự thảo), 2=Approved (Đã duyệt), 3=InProgress (Đang thực hiện), 4=Completed (Hoàn thành), 5=Cancelled (Đã hủy)',
    `TotalRequiredQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Tổng nhu cầu cần sản xuất',
    `TotalPlannedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Tổng số lượng đã lập kế hoạch',
    `ProductionOrderCount` INT NOT NULL DEFAULT 0 COMMENT 'Số lệnh sản xuất đã sinh',
    `Remark` TEXT NULL COMMENT 'Ghi chú',

    UNIQUE KEY `UX_detailed_prod_plan_code` (`PlanCode`),
    KEY `IX_detailed_prod_plan_master_id` (`MasterPlanId`),
    KEY `IX_detailed_prod_plan_status` (`Status`),
    KEY `IX_detailed_prod_plan_from_date` (`FromDate`),
    KEY `IX_detailed_prod_plan_to_date` (`ToDate`),
    KEY `IX_detailed_prod_plan_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Kế hoạch sản xuất chi tiết (Detailed Production Plan)';

-- 4. Tổng hợp nhu cầu sản phẩm của Kế hoạch chi tiết
CREATE TABLE IF NOT EXISTS `detailed_production_plan_item` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CreatedBy` VARCHAR(50) NOT NULL,
    `UpdatedDate` DATETIME NULL,
    `UpdatedBy` VARCHAR(50) NULL,
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0,

    `PlanId` BIGINT UNSIGNED NOT NULL COMMENT 'ID kế hoạch chi tiết',
    `ProductId` BIGINT UNSIGNED NOT NULL COMMENT 'ID sản phẩm',
    `ProductCode` VARCHAR(50) NOT NULL COMMENT 'Mã sản phẩm',
    `ProductName` VARCHAR(255) NOT NULL COMMENT 'Tên sản phẩm',
    `UnitId` BIGINT UNSIGNED NULL,
    `UnitName` VARCHAR(50) NULL,
    `RequiredQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Số lượng nhu cầu cần sản xuất',
    `CurrentStock` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Tồn kho hiện tại',
    `PlannedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Số lượng đã phân bổ trong bảng',
    `DailyCapacity` DECIMAL(18, 4) NOT NULL DEFAULT 100.0000 COMMENT 'Công suất sản xuất định mức / ngày',
    `SalesOrderNo` VARCHAR(100) NULL COMMENT 'Mã đơn đặt hàng liên quan',
    `Sequence` INT NOT NULL DEFAULT 1,
    `Remark` VARCHAR(255) NULL,

    KEY `IX_det_plan_item_plan_id` (`PlanId`),
    KEY `IX_det_plan_item_product_id` (`ProductId`),
    KEY `IX_det_plan_item_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Tổng hợp nhu cầu sản phẩm của Kế hoạch chi tiết';

-- 5. Lịch phân bổ sản xuất theo ngày / ca
CREATE TABLE IF NOT EXISTS `detailed_production_plan_schedule` (
    `Id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
    `CreatedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `CreatedBy` VARCHAR(50) NOT NULL,
    `UpdatedDate` DATETIME NULL,
    `UpdatedBy` VARCHAR(50) NULL,
    `IsDeleted` TINYINT(1) NOT NULL DEFAULT 0,

    `PlanId` BIGINT UNSIGNED NOT NULL COMMENT 'ID kế hoạch chi tiết',
    `PlanItemId` BIGINT UNSIGNED NOT NULL COMMENT 'ID dòng nhu cầu sản phẩm',
    `ProductId` BIGINT UNSIGNED NOT NULL COMMENT 'ID sản phẩm',
    `ScheduleDate` DATE NOT NULL COMMENT 'Ngày phân bổ sản xuất',
    `ShiftId` BIGINT UNSIGNED NULL COMMENT 'ID ca làm việc',
    `ShiftName` VARCHAR(50) NULL COMMENT 'Tên ca làm việc (Ca 1, Ca 2, Ca 3)',
    `Capacity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Công suất khả dụng',
    `PlannedQuantity` DECIMAL(18, 4) NOT NULL DEFAULT 0.0000 COMMENT 'Số lượng sản xuất dự kiến',
    `IsDayOff` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Ngày nghỉ',
    `IsOvertime` TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Tăng ca',
    `ProductionOrderId` BIGINT UNSIGNED NULL COMMENT 'ID Lệnh sản xuất đã sinh',
    `ProductionOrderNo` VARCHAR(50) NULL COMMENT 'Mã Lệnh sản xuất đã sinh',
    `Remark` VARCHAR(255) NULL,

    KEY `IX_det_plan_sch_plan_id` (`PlanId`),
    KEY `IX_det_plan_sch_item_id` (`PlanItemId`),
    KEY `IX_det_plan_sch_date` (`ScheduleDate`),
    KEY `IX_det_plan_sch_prod_order` (`ProductionOrderId`),
    KEY `IX_det_plan_sch_is_deleted` (`IsDeleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Lịch phân bổ sản xuất theo ngày/ca';

-- Seed sample Master Production Plan for testing
INSERT INTO `master_production_plan` (`Id`, `CreatedDate`, `CreatedBy`, `PlanCode`, `PlanName`, `PeriodType`, `PeriodName`, `FromDate`, `ToDate`, `WorkshopName`, `Status`, `TotalQuantity`, `Remark`)
VALUES
(1, NOW(), 'admin', 'KHTT-2026-T10', 'Kế hoạch sản xuất tổng thể Tháng 10/2026', 'MONTH', 'Tháng 10/2026', '2026-10-01', '2026-10-31', 'Xưởng Lắp Ráp 1', 2, 500.0000, 'Kế hoạch tổng thể phục vụ đơn hàng quý 4/2026'),
(2, NOW(), 'admin', 'KHTT-2026-T11', 'Kế hoạch sản xuất tổng thể Tháng 11/2026', 'MONTH', 'Tháng 11/2026', '2026-11-01', '2026-11-30', 'Xưởng Cơ Khí 2', 2, 800.0000, 'Kế hoạch dự phòng mở rộng công suất')
ON DUPLICATE KEY UPDATE `PlanName` = VALUES(`PlanName`);

INSERT INTO `master_production_plan_item` (`Id`, `CreatedDate`, `CreatedBy`, `MasterPlanId`, `ProductId`, `ProductCode`, `ProductName`, `UnitName`, `PlannedQuantity`, `AllocatedQuantity`, `RemainingQuantity`, `DailyCapacity`)
VALUES
(1, NOW(), 'admin', 1, 1, 'PDA-A23', 'PDA mã hiệu A23', 'Chiếc', 300.0000, 0.0000, 300.0000, 50.0000),
(2, NOW(), 'admin', 1, 2, 'BOARD-X1', 'Bo mạch điều khiển X1', 'Bộ', 200.0000, 0.0000, 200.0000, 40.0000)
ON DUPLICATE KEY UPDATE `PlannedQuantity` = VALUES(`PlannedQuantity`);
