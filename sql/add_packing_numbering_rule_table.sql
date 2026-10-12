-- ============================================================
-- Migration: Add packing_numbering_rule table & default rules
-- Run once against the RFactory MySQL database.
-- ============================================================

CREATE TABLE IF NOT EXISTS `packing_numbering_rule` (
  `id`                   BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `created_date`         DATETIME          NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by`           VARCHAR(100)      NULL,
  `updated_date`         DATETIME          NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by`           VARCHAR(100)      NULL,
  `is_deleted`           TINYINT UNSIGNED  NOT NULL DEFAULT 0,

  `rule_code`            VARCHAR(50)       NOT NULL,
  `rule_name`            VARCHAR(200)      NOT NULL,
  `target_scope`         VARCHAR(50)       NOT NULL DEFAULT 'CARTON', -- CARTON, PALLET, INNER_BOX, SPEC, LOT

  `prefix`               VARCHAR(50)       NULL,
  `suffix`               VARCHAR(50)       NULL,
  `date_format`          VARCHAR(20)       NOT NULL DEFAULT 'YYYYMMDD', -- NONE, YYYYMMDD, YYMMDD, YYYYMM, YYWW
  `sequence_length`      INT               NOT NULL DEFAULT 4,
  `current_sequence`     BIGINT            NOT NULL DEFAULT 0,
  `reset_period`         VARCHAR(20)       NOT NULL DEFAULT 'DAILY', -- NEVER, DAILY, MONTHLY, YEARLY
  `last_reset_date`      DATETIME          NULL,

  `include_check_digit`  TINYINT UNSIGNED  NOT NULL DEFAULT 0,
  `is_default`           TINYINT UNSIGNED  NOT NULL DEFAULT 0,
  `is_active`            TINYINT UNSIGNED  NOT NULL DEFAULT 1,
  `description`          VARCHAR(500)      NULL,

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_packing_numbering_rule_code` (`rule_code`, `is_deleted`),
  KEY `ix_packing_numbering_rule_scope` (`target_scope`, `is_default`, `is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ── Default Seed Rules ──────────────────────────────────────
INSERT INTO `packing_numbering_rule`
(
  `rule_code`, `rule_name`, `target_scope`, `prefix`, `suffix`,
  `date_format`, `sequence_length`, `current_sequence`, `reset_period`,
  `include_check_digit`, `is_default`, `is_active`, `description`
)
VALUES
(
  'NR-CARTON-DAILY',
  'Master Carton Daily Sequential (CTN-YYYYMMDD-####)',
  'CARTON',
  'CTN-',
  NULL,
  'YYYYMMDD',
  4,
  0,
  'DAILY',
  0,
  1,
  1,
  'Standard master carton barcode rule reset daily at midnight (e.g. CTN-20261012-0001)'
),
(
  'NR-PALLET-SSCC',
  'Pallet Container Code (PLT-YYMMDD-#####)',
  'PALLET',
  'PLT-',
  NULL,
  'YYMMDD',
  5,
  0,
  'DAILY',
  1,
  1,
  1,
  'Pallet container tracking code with Modulo 10 check digit (e.g. PLT-261012-000017)'
),
(
  'NR-BOX-MONTHLY',
  'Inner Box Monthly Sequence (BOX-YYYYMM-####)',
  'INNER_BOX',
  'BOX-',
  NULL,
  'YYYYMM',
  4,
  0,
  'MONTHLY',
  0,
  1,
  1,
  'Inner box packaging code reset on the 1st of each month'
),
(
  'NR-SPEC-SEQ',
  'Packing Specification Code (PK-SPEC-####)',
  'SPEC',
  'PK-SPEC-',
  NULL,
  'NONE',
  4,
  2,
  'NEVER',
  0,
  1,
  1,
  'Global auto-increment sequence for packing specification master records'
)
ON DUPLICATE KEY UPDATE `rule_name` = VALUES(`rule_name`);
