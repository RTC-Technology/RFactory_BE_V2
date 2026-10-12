-- ============================================================
-- Migration: Add machine_reason_code, machine_status_history, 
-- and machine_defect_log tables & seeds for MES/OEE tracking
-- ============================================================

-- 1. Reason Codes (Downtime, Scrap, Rework)
CREATE TABLE IF NOT EXISTS `machine_reason_code` (
  `id`              BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `created_date`    DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by`      VARCHAR(100)     NULL,
  `updated_date`    DATETIME         NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by`      VARCHAR(100)     NULL,
  `is_deleted`      TINYINT UNSIGNED NOT NULL DEFAULT 0,

  `reason_code`     VARCHAR(50)      NOT NULL,
  `reason_name`     VARCHAR(200)     NOT NULL,
  `reason_type`     VARCHAR(50)      NOT NULL DEFAULT 'DOWNTIME', -- DOWNTIME, SCRAP, REWORK
  `category`        VARCHAR(100)     NOT NULL DEFAULT 'General',  -- Mechanical, Electrical, Material, Tooling, Quality, Process, Operational
  `is_planned`      TINYINT UNSIGNED NOT NULL DEFAULT 0,          -- 1 for planned (PM, Setup), 0 for unplanned breakdowns
  `is_active`       TINYINT UNSIGNED NOT NULL DEFAULT 1,
  `description`     VARCHAR(500)     NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_machine_reason_code` (`reason_code`, `is_deleted`),
  KEY `ix_machine_reason_type` (`reason_type`, `is_active`, `is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- 2. Machine Status History (State transitions & active status)
CREATE TABLE IF NOT EXISTS `machine_status_history` (
  `id`                BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `created_date`      DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by`        VARCHAR(100)     NULL,
  `updated_date`      DATETIME         NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by`        VARCHAR(100)     NULL,
  `is_deleted`        TINYINT UNSIGNED NOT NULL DEFAULT 0,

  `machine_id`        BIGINT UNSIGNED  NOT NULL,
  `status`            VARCHAR(50)      NOT NULL DEFAULT 'IDLE', -- RUNNING, IDLE, DOWNTIME, CHANGEOVER, MAINTENANCE, OFFLINE
  `start_time`        DATETIME         NOT NULL,
  `end_time`          DATETIME         NULL,
  `duration_seconds`  BIGINT           NULL,

  `reason_id`         BIGINT UNSIGNED  NULL,
  `reason_code`       VARCHAR(50)      NULL,
  `reason_name`       VARCHAR(200)     NULL,
  `operator_name`     VARCHAR(100)     NULL,
  `work_order_code`   VARCHAR(100)     NULL,
  `remarks`           VARCHAR(500)     NULL,

  PRIMARY KEY (`id`),
  KEY `ix_msh_machine_active` (`machine_id`, `end_time`, `is_deleted`),
  KEY `ix_msh_machine_time` (`machine_id`, `start_time`, `is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- 3. Machine Defect Log (Scrap & Rework counts)
CREATE TABLE IF NOT EXISTS `machine_defect_log` (
  `id`                BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `created_date`      DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by`        VARCHAR(100)     NULL,
  `updated_date`      DATETIME         NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by`        VARCHAR(100)     NULL,
  `is_deleted`        TINYINT UNSIGNED NOT NULL DEFAULT 0,

  `machine_id`        BIGINT UNSIGNED  NOT NULL,
  `defect_type`       VARCHAR(50)      NOT NULL DEFAULT 'SCRAP', -- SCRAP, REWORK
  `reason_id`         BIGINT UNSIGNED  NULL,
  `reason_code`       VARCHAR(50)      NULL,
  `reason_name`       VARCHAR(200)     NULL,
  `quantity`          DECIMAL(12, 4)   NOT NULL DEFAULT 1.0000,
  `product_id`        BIGINT UNSIGNED  NULL,
  `product_code`      VARCHAR(50)      NULL,
  `work_order_code`   VARCHAR(100)     NULL,
  `logged_time`       DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `operator_name`     VARCHAR(100)     NULL,
  `remarks`           VARCHAR(500)     NULL,

  PRIMARY KEY (`id`),
  KEY `ix_mdl_machine_time` (`machine_id`, `logged_time`, `is_deleted`),
  KEY `ix_mdl_type_time` (`defect_type`, `logged_time`, `is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;


-- ── Standard Reason Code Seeds ──────────────────────────────

-- Downtime Reasons
INSERT INTO `machine_reason_code` 
(`reason_code`, `reason_name`, `reason_type`, `category`, `is_planned`, `is_active`, `description`)
VALUES
('DT-MECH-01',  'Mechanical Breakdown',             'DOWNTIME', 'Mechanical',  0, 1, 'Component breakdown, motor failure, mechanical jam or fracture'),
('DT-ELEC-01',  'Electrical / Sensor Fault',        'DOWNTIME', 'Electrical',  0, 1, 'PLC alarm, proximity sensor error, wire harness defect'),
('DT-TOOL-01',  'Tooling Damage or Wear',           'DOWNTIME', 'Tooling',     0, 1, 'Die/mold wear, blade fracture, fixture misalignment'),
('DT-MAT-01',   'Material Starvation / Shortage',   'DOWNTIME', 'Material',    0, 1, 'Raw material exhausted, waiting for warehouse delivery'),
('DT-SETUP-01', 'Setup & Mold Changeover',          'DOWNTIME', 'Changeover',  1, 1, 'Planned tooling changeover, calibration, first article inspection'),
('DT-PM-01',    'Scheduled PM Maintenance',         'DOWNTIME', 'Maintenance', 1, 1, 'Periodic preventive lubrication, cleaning, and inspection'),
('DT-QC-01',    'Quality Hold / Defect Stop',       'DOWNTIME', 'Quality',     0, 1, 'Line stop triggered by continuous defects or SPC violation'),
('DT-OPR-01',   'Shift Change / Operator Break',    'DOWNTIME', 'Operational', 1, 1, 'Planned lunch break or shift handover'),
('DT-UTIL-01',  'Pneumatic / Utility Outage',       'DOWNTIME', 'Utility',     0, 1, 'Low air pressure, power fluctuation, chiller temperature alert'),

-- Scrap Reasons (Defect / Unrecoverable)
('SCRAP-DIM-01',  'Dimension Out of Spec',          'SCRAP', 'Quality',    0, 1, 'Part length, width, or thickness beyond allowable tolerance'),
('SCRAP-SURF-01', 'Surface Dent / Scratch',         'SCRAP', 'Quality',    0, 1, 'Visible scratch, dent, or cosmetic surface blemish'),
('SCRAP-MAT-01',  'Material Porosity / Blister',    'SCRAP', 'Material',   0, 1, 'Substandard raw material with voids, porosity, or inclusions'),
('SCRAP-BURN-01', 'Burn / Thermal Degradation',     'SCRAP', 'Process',    0, 1, 'Overheated plastic melt, scorched coating, thermal deformation'),
('SCRAP-START-01','Startup Purge Scrap',            'SCRAP', 'Process',    0, 1, 'Initial warmup parts discarded before reaching steady state'),

-- Rework Reasons (Repairable)
('REW-BURR-01',   'Excess Flash / Burr',            'REWORK', 'Mechanical', 0, 1, 'Flash along parting line requiring manual trimming/deburring'),
('REW-ALIGN-01',  'Component Misalignment',         'REWORK', 'Assembly',   0, 1, 'Improperly positioned bracket or clip requiring re-alignment'),
('REW-COAT-01',   'Touch-up Paint / Coating',       'REWORK', 'Finishing',  0, 1, 'Light uneven paint coverage requiring secondary spray touch-up'),
('REW-ELEC-01',   'Re-solder / Re-seat Connector',  'REWORK', 'Electrical', 0, 1, 'Cold solder joint or unseated pin connector requiring rework')
ON DUPLICATE KEY UPDATE `reason_name` = VALUES(`reason_name`);
