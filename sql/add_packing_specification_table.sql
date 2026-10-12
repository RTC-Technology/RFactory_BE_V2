-- ============================================================
-- Migration: Add packing_specification table & initial seeds
-- Run once against the RFactory MySQL database.
-- ============================================================

CREATE TABLE IF NOT EXISTS `packing_specification` (
  `id`                     BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `created_date`           DATETIME          NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by`             VARCHAR(100)      NULL,
  `updated_date`           DATETIME          NULL     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by`             VARCHAR(100)      NULL,
  `is_deleted`             TINYINT UNSIGNED  NOT NULL DEFAULT 0,

  `spec_code`              VARCHAR(50)       NOT NULL,
  `spec_name`              VARCHAR(200)      NOT NULL,
  `product_id`             BIGINT UNSIGNED   NULL,
  `customer_code`          VARCHAR(100)      NULL,
  `customer_name`          VARCHAR(200)      NULL,
  `packaging_type`         VARCHAR(50)       NOT NULL DEFAULT 'CARTON', -- BOX, CARTON, PALLET, CRATE, BUNDLE
  `quantity_per_pack`      DECIMAL(12,4)     NOT NULL DEFAULT 1.0000,
  `pack_unit_id`           BIGINT UNSIGNED   NULL,

  `length_mm`              DECIMAL(8,2)      NOT NULL DEFAULT 0.00,
  `width_mm`               DECIMAL(8,2)      NOT NULL DEFAULT 0.00,
  `height_mm`              DECIMAL(8,2)      NOT NULL DEFAULT 0.00,
  `volume_m3`              DECIMAL(10,4)     NOT NULL DEFAULT 0.0000,
  `net_weight_kg`          DECIMAL(8,3)      NOT NULL DEFAULT 0.000,
  `gross_weight_kg`        DECIMAL(8,3)      NOT NULL DEFAULT 0.000,
  `tare_weight_kg`         DECIMAL(8,3)      NOT NULL DEFAULT 0.000,

  `inner_pack_qty`         DECIMAL(12,4)     NULL,
  `master_carton_qty`      DECIMAL(12,4)     NULL,
  `pallet_ti`              INT               NULL, -- Cartons per layer
  `pallet_hi`              INT               NULL, -- Layers per pallet
  `total_units_per_pallet` DECIMAL(12,4)     NULL,

  `label_template_id`      BIGINT UNSIGNED   NULL,
  `barcode`                VARCHAR(100)      NULL,
  `packaging_material`     VARCHAR(200)      NULL,
  `instructions`           TEXT              NULL,
  `is_default`             TINYINT UNSIGNED  NOT NULL DEFAULT 0,
  `status`                 VARCHAR(30)       NOT NULL DEFAULT 'ACTIVE', -- ACTIVE, DRAFT, OBSOLETE

  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_packing_spec_code` (`spec_code`, `is_deleted`),
  KEY `ix_packing_spec_product` (`product_id`, `is_default`, `is_deleted`),
  KEY `ix_packing_spec_status` (`status`, `is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ── Initial Seeds ───────────────────────────────────────────
INSERT INTO `packing_specification` 
(
  `spec_code`, `spec_name`, `packaging_type`, `quantity_per_pack`,
  `length_mm`, `width_mm`, `height_mm`, `volume_m3`,
  `net_weight_kg`, `gross_weight_kg`, `tare_weight_kg`,
  `inner_pack_qty`, `master_carton_qty`, `pallet_ti`, `pallet_hi`, `total_units_per_pallet`,
  `packaging_material`, `instructions`, `is_default`, `status`
)
VALUES
(
  'PK-STD-BOX-01',
  'Standard Master Carton Specification (50 pcs/carton)',
  'CARTON',
  50.0000,
  450.00, 320.00, 280.00, 0.0403,
  12.500, 13.800, 1.300,
  10.0000, 5.0000, 8, 4, 1600.0000,
  '5-ply Corrugated Carton, Polybags, Silica Gel Desiccant 25g',
  'Packed 10 pcs per polybag, 5 bags per outer carton. Affix QR shipping label on top right.',
  1,
  'ACTIVE'
),
(
  'PK-EXP-PALLET-02',
  'Export Standard Pallet Stacking (Euro Pallet 1200x800)',
  'PALLET',
  1200.0000,
  1200.00, 800.00, 1450.00, 1.3920,
  350.000, 385.000, 35.000,
  50.0000, 24.0000, 6, 4, 1200.0000,
  'Heat-treated Wooden Pallet (ISPM-15), Corner Protectors, Stretch Film 23um',
  'Stack Ti:6 x Hi:4 cartons. Wrap with stretch film min 4 layers, strap securely.',
  0,
  'ACTIVE'
)
ON DUPLICATE KEY UPDATE `spec_name` = VALUES(`spec_name`);
