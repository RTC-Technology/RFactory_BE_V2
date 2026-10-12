-- ============================================================
-- Migration: Add Position, Skill, EmployeeSkill tables
-- Run once against the RFactory database.
-- ============================================================

-- ── 1. Position ─────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `position` (
    `id`            BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `created_date`  DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `created_by`    VARCHAR(100)    NULL,
    `updated_date`  DATETIME        NULL,
    `updated_by`    VARCHAR(100)    NULL,
    `is_deleted`    TINYINT(1)      NOT NULL DEFAULT 0,
    `position_code` VARCHAR(50)     NOT NULL,
    `position_name` VARCHAR(200)    NOT NULL,
    `department`    VARCHAR(100)    NULL,
    `description`   VARCHAR(500)    NULL,
    `is_active`     TINYINT(1)      NULL DEFAULT 1,
    PRIMARY KEY (`id`),
    UNIQUE KEY `UQ_position_code` (`position_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Job title / position master data';

-- ── 2. Skill ────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `skill` (
    `id`             BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `created_date`   DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `created_by`     VARCHAR(100)    NULL,
    `updated_date`   DATETIME        NULL,
    `updated_by`     VARCHAR(100)    NULL,
    `is_deleted`     TINYINT(1)      NOT NULL DEFAULT 0,
    `skill_code`     VARCHAR(50)     NOT NULL,
    `skill_name`     VARCHAR(200)    NOT NULL,
    `skill_category` VARCHAR(100)    NULL,
    `description`    VARCHAR(500)    NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `UQ_skill_code` (`skill_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Skill catalogue';

-- ── 3. EmployeeSkill ─────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `employee_skill` (
    `id`                BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `created_date`      DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `created_by`        VARCHAR(100)    NULL,
    `updated_date`      DATETIME        NULL,
    `updated_by`        VARCHAR(100)    NULL,
    `is_deleted`        TINYINT(1)      NOT NULL DEFAULT 0,
    `employee_id`       BIGINT UNSIGNED NOT NULL,
    `skill_id`          BIGINT UNSIGNED NOT NULL,
    `proficiency_level` INT             NULL COMMENT '1=Beginner 2=Elementary 3=Intermediate 4=Advanced 5=Expert',
    `acquired_date`     DATE            NULL,
    `notes`             VARCHAR(500)    NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `UQ_employee_skill` (`employee_id`, `skill_id`),
    KEY `IX_employee_skill_employee_id` (`employee_id`),
    KEY `IX_employee_skill_skill_id`    (`skill_id`),
    CONSTRAINT `FK_employee_skill_employee` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`id`),
    CONSTRAINT `FK_employee_skill_skill`    FOREIGN KEY (`skill_id`)    REFERENCES `skill`    (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Employee-skill assignments with optional proficiency rating';
