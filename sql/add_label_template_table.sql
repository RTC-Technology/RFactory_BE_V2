-- ============================================================
-- Migration: Add label_template table & default templates
-- Run once against the RFactory MySQL database.
-- ============================================================

CREATE TABLE IF NOT EXISTS `label_template` (
  `id`                 BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `created_date`       DATETIME         NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by`         VARCHAR(100)     NULL,
  `updated_date`       DATETIME         NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by`         VARCHAR(100)     NULL,
  `is_deleted`         TINYINT UNSIGNED NOT NULL DEFAULT 0,

  `template_code`      VARCHAR(50)      NOT NULL,
  `template_name`      VARCHAR(200)     NOT NULL,
  `entity_type`        VARCHAR(50)      NOT NULL, -- PRODUCT, MACHINE, LOCATION, EMPLOYEE
  `width_mm`           DECIMAL(6,2)     NOT NULL DEFAULT 70.00,
  `height_mm`          DECIMAL(6,2)     NOT NULL DEFAULT 50.00,
  `orientation`        VARCHAR(20)      NOT NULL DEFAULT 'LANDSCAPE',
  `is_default`         TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `is_active`          TINYINT UNSIGNED NOT NULL DEFAULT 1,
  `header_text`        VARCHAR(200)     NULL,
  `footer_text`        VARCHAR(200)     NULL,
  `show_qr`            TINYINT UNSIGNED NOT NULL DEFAULT 1,
  `show_barcode`       TINYINT UNSIGNED NOT NULL DEFAULT 0,
  `show_logo`          TINYINT UNSIGNED NOT NULL DEFAULT 1,
  `show_border`        TINYINT UNSIGNED NOT NULL DEFAULT 1,
  `qr_size_px`         INT              NOT NULL DEFAULT 150,
  `fields_config_json` LONGTEXT         NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_label_template_code` (`template_code`, `is_deleted`),
  KEY `ix_label_template_entity` (`entity_type`, `is_default`, `is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ── Default Seed Templates ──────────────────────────────────
INSERT INTO `label_template` 
(`template_code`, `template_name`, `entity_type`, `width_mm`, `height_mm`, `orientation`, `is_default`, `is_active`, `header_text`, `footer_text`, `show_qr`, `show_barcode`, `show_logo`, `show_border`, `qr_size_px`, `fields_config_json`)
VALUES
(
  'TMPL-PROD-STD',
  'Product Standard Label (70x50mm)',
  'PRODUCT',
  70.00,
  50.00,
  'LANDSCAPE',
  1,
  1,
  'R-FACTORY MES | PRODUCT SPEC',
  'QC INSPECTED & CERTIFIED',
  1,
  0,
  1,
  1,
  140,
  '[{"fieldKey":"code","label":"SKU / Code","fontSize":13,"bold":true,"visible":true},{"fieldKey":"name","label":"Product Name","fontSize":14,"bold":true,"visible":true},{"fieldKey":"spec","label":"Specification","fontSize":11,"bold":false,"visible":true},{"fieldKey":"date","label":"Mfg Date","fontSize":10,"bold":false,"visible":true}]'
),
(
  'TMPL-LOC-BIN',
  'Warehouse Bin & Location Tag (100x50mm)',
  'LOCATION',
  100.00,
  50.00,
  'LANDSCAPE',
  1,
  1,
  'R-FACTORY LOGISTICS | STORAGE LOCATION',
  'ZONE SHELF BIN RACK IDENTIFIER',
  1,
  1,
  1,
  1,
  150,
  '[{"fieldKey":"code","label":"Location Code","fontSize":16,"bold":true,"visible":true},{"fieldKey":"name","label":"Location Name","fontSize":14,"bold":true,"visible":true},{"fieldKey":"zone","label":"Zone Context","fontSize":12,"bold":false,"visible":true},{"fieldKey":"capacity","label":"Max Capacity","fontSize":11,"bold":false,"visible":true}]'
),
(
  'TMPL-MAC-ASSET',
  'Machine Asset Tag (80x40mm)',
  'MACHINE',
  80.00,
  40.00,
  'LANDSCAPE',
  1,
  1,
  'R-FACTORY EQUIPMENT ASSET',
  'PERIODIC OEE MAINTENANCE INSPECTED',
  1,
  0,
  1,
  1,
  130,
  '[{"fieldKey":"code","label":"Asset #","fontSize":13,"bold":true,"visible":true},{"fieldKey":"name","label":"Machine Name","fontSize":14,"bold":true,"visible":true},{"fieldKey":"model","label":"Model / Serial","fontSize":11,"bold":false,"visible":true},{"fieldKey":"line","label":"Production Line","fontSize":11,"bold":false,"visible":true}]'
),
(
  'TMPL-EMP-BADGE',
  'Employee ID Badge (85x54mm)',
  'EMPLOYEE',
  85.00,
  54.00,
  'PORTRAIT',
  1,
  1,
  'R-FACTORY PERSONNEL BADGE',
  'AUTHORIZED PLANT OPERATOR',
  1,
  0,
  1,
  1,
  130,
  '[{"fieldKey":"code","label":"Employee ID","fontSize":12,"bold":true,"visible":true},{"fieldKey":"name","label":"Full Name","fontSize":15,"bold":true,"visible":true},{"fieldKey":"position","label":"Position / Title","fontSize":12,"bold":false,"visible":true},{"fieldKey":"department","label":"Department","fontSize":11,"bold":false,"visible":true}]'
)
ON DUPLICATE KEY UPDATE `template_name` = VALUES(`template_name`);
