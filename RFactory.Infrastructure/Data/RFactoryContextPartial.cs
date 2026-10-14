using Microsoft.EntityFrameworkCore;
using RFactory.Infrastructure.Entities;

namespace RFactory.Infrastructure.Data;

/// <summary>
/// Extends the auto-generated <see cref="RFactoryContext"/> with DbSets for entities
/// that were added manually (not scaffolded from an existing table).
/// </summary>
public partial class RFactoryContext
{
    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<EmployeeSkill> EmployeeSkills { get; set; }

    public virtual DbSet<LabelTemplate> LabelTemplates { get; set; }

    public virtual DbSet<PackingSpecification> PackingSpecifications { get; set; }

    public virtual DbSet<PackingNumberingRule> PackingNumberingRules { get; set; }

    public virtual DbSet<MachineReasonCode> MachineReasonCodes { get; set; }

    public virtual DbSet<MachineStatusHistory> MachineStatusHistories { get; set; }

    public virtual DbSet<MachineDefectLog> MachineDefectLogs { get; set; }

    public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }

    public virtual DbSet<ProductionOrderMaterial> ProductionOrderMaterials { get; set; }

    public virtual DbSet<ProductionOrderOperation> ProductionOrderOperations { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
        });

        // ── Position ────────────────────────────────────────────────────────
        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("position");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PositionCode).HasColumnName("position_code").HasMaxLength(50);
            entity.Property(e => e.PositionName).HasColumnName("position_name").HasMaxLength(200);
            entity.Property(e => e.Department).HasColumnName("department").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.IsActive).HasColumnName("is_active");
        });

        // ── Skill ────────────────────────────────────────────────────────────
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("skill");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.SkillCode).HasColumnName("skill_code").HasMaxLength(50);
            entity.Property(e => e.SkillName).HasColumnName("skill_name").HasMaxLength(200);
            entity.Property(e => e.SkillCategory).HasColumnName("skill_category").HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        });

        // ── EmployeeSkill ────────────────────────────────────────────────────
        modelBuilder.Entity<EmployeeSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("employee_skill");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.SkillId).HasColumnName("skill_id");
            entity.Property(e => e.ProficiencyLevel).HasColumnName("proficiency_level");
            entity.Property(e => e.AcquiredDate).HasColumnName("acquired_date");
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(500);
        });

        // ── LabelTemplate ────────────────────────────────────────────────────
        modelBuilder.Entity<LabelTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("label_template");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.TemplateCode).HasColumnName("template_code").HasMaxLength(50);
            entity.Property(e => e.TemplateName).HasColumnName("template_name").HasMaxLength(200);
            entity.Property(e => e.EntityType).HasColumnName("entity_type").HasMaxLength(50);
            entity.Property(e => e.WidthMm).HasColumnName("width_mm").HasPrecision(6, 2);
            entity.Property(e => e.HeightMm).HasColumnName("height_mm").HasPrecision(6, 2);
            entity.Property(e => e.Orientation).HasColumnName("orientation").HasMaxLength(20);
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.HeaderText).HasColumnName("header_text").HasMaxLength(200);
            entity.Property(e => e.FooterText).HasColumnName("footer_text").HasMaxLength(200);
            entity.Property(e => e.ShowQr).HasColumnName("show_qr");
            entity.Property(e => e.ShowBarcode).HasColumnName("show_barcode");
            entity.Property(e => e.ShowLogo).HasColumnName("show_logo");
            entity.Property(e => e.ShowBorder).HasColumnName("show_border");
            entity.Property(e => e.QrSizePx).HasColumnName("qr_size_px");
            entity.Property(e => e.FieldsConfigJson).HasColumnName("fields_config_json");
        });

        // ── PackingSpecification ─────────────────────────────────────────────
        modelBuilder.Entity<PackingSpecification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("packing_specification");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.SpecCode).HasColumnName("spec_code").HasMaxLength(50);
            entity.Property(e => e.SpecName).HasColumnName("spec_name").HasMaxLength(200);
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CustomerCode).HasColumnName("customer_code").HasMaxLength(100);
            entity.Property(e => e.CustomerName).HasColumnName("customer_name").HasMaxLength(200);
            entity.Property(e => e.PackagingType).HasColumnName("packaging_type").HasMaxLength(50);
            entity.Property(e => e.QuantityPerPack).HasColumnName("quantity_per_pack").HasPrecision(12, 4);
            entity.Property(e => e.PackUnitId).HasColumnName("pack_unit_id");
            entity.Property(e => e.LengthMm).HasColumnName("length_mm").HasPrecision(8, 2);
            entity.Property(e => e.WidthMm).HasColumnName("width_mm").HasPrecision(8, 2);
            entity.Property(e => e.HeightMm).HasColumnName("height_mm").HasPrecision(8, 2);
            entity.Property(e => e.VolumeM3).HasColumnName("volume_m3").HasPrecision(10, 4);
            entity.Property(e => e.NetWeightKg).HasColumnName("net_weight_kg").HasPrecision(8, 3);
            entity.Property(e => e.GrossWeightKg).HasColumnName("gross_weight_kg").HasPrecision(8, 3);
            entity.Property(e => e.TareWeightKg).HasColumnName("tare_weight_kg").HasPrecision(8, 3);
            entity.Property(e => e.InnerPackQty).HasColumnName("inner_pack_qty").HasPrecision(12, 4);
            entity.Property(e => e.MasterCartonQty).HasColumnName("master_carton_qty").HasPrecision(12, 4);
            entity.Property(e => e.PalletTi).HasColumnName("pallet_ti");
            entity.Property(e => e.PalletHi).HasColumnName("pallet_hi");
            entity.Property(e => e.TotalUnitsPerPallet).HasColumnName("total_units_per_pallet").HasPrecision(12, 4);
            entity.Property(e => e.LabelTemplateId).HasColumnName("label_template_id");
            entity.Property(e => e.Barcode).HasColumnName("barcode").HasMaxLength(100);
            entity.Property(e => e.PackagingMaterial).HasColumnName("packaging_material").HasMaxLength(200);
            entity.Property(e => e.Instructions).HasColumnName("instructions");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(30);
        });

        // ── PackingNumberingRule ─────────────────────────────────────────────
        modelBuilder.Entity<PackingNumberingRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("packing_numbering_rule");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.RuleCode).HasColumnName("rule_code").HasMaxLength(50);
            entity.Property(e => e.RuleName).HasColumnName("rule_name").HasMaxLength(200);
            entity.Property(e => e.TargetScope).HasColumnName("target_scope").HasMaxLength(50);
            entity.Property(e => e.Prefix).HasColumnName("prefix").HasMaxLength(50);
            entity.Property(e => e.Suffix).HasColumnName("suffix").HasMaxLength(50);
            entity.Property(e => e.DateFormat).HasColumnName("date_format").HasMaxLength(20);
            entity.Property(e => e.SequenceLength).HasColumnName("sequence_length");
            entity.Property(e => e.CurrentSequence).HasColumnName("current_sequence");
            entity.Property(e => e.ResetPeriod).HasColumnName("reset_period").HasMaxLength(20);
            entity.Property(e => e.LastResetDate).HasColumnName("last_reset_date");
            entity.Property(e => e.IncludeCheckDigit).HasColumnName("include_check_digit");
            entity.Property(e => e.IsDefault).HasColumnName("is_default");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        });

        // ── MachineReasonCode ────────────────────────────────────────────────
        modelBuilder.Entity<MachineReasonCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("machine_reason_code");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ReasonCode).HasColumnName("reason_code").HasMaxLength(50);
            entity.Property(e => e.ReasonName).HasColumnName("reason_name").HasMaxLength(200);
            entity.Property(e => e.ReasonType).HasColumnName("reason_type").HasMaxLength(50);
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100);
            entity.Property(e => e.IsPlanned).HasColumnName("is_planned");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
        });

        // ── MachineStatusHistory ─────────────────────────────────────────────
        modelBuilder.Entity<MachineStatusHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("machine_status_history");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.DurationSeconds).HasColumnName("duration_seconds");
            entity.Property(e => e.ReasonId).HasColumnName("reason_id");
            entity.Property(e => e.ReasonCode).HasColumnName("reason_code").HasMaxLength(50);
            entity.Property(e => e.ReasonName).HasColumnName("reason_name").HasMaxLength(200);
            entity.Property(e => e.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            entity.Property(e => e.WorkOrderCode).HasColumnName("work_order_code").HasMaxLength(100);
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
        });

        // ── MachineDefectLog ─────────────────────────────────────────────────
        modelBuilder.Entity<MachineDefectLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("machine_defect_log");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by").HasMaxLength(100);
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MachineId).HasColumnName("machine_id");
            entity.Property(e => e.DefectType).HasColumnName("defect_type").HasMaxLength(50);
            entity.Property(e => e.ReasonId).HasColumnName("reason_id");
            entity.Property(e => e.ReasonCode).HasColumnName("reason_code").HasMaxLength(50);
            entity.Property(e => e.ReasonName).HasColumnName("reason_name").HasMaxLength(200);
            entity.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(12, 4);
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductCode).HasColumnName("product_code").HasMaxLength(50);
            entity.Property(e => e.WorkOrderCode).HasColumnName("work_order_code").HasMaxLength(100);
            entity.Property(e => e.LoggedTime).HasColumnName("logged_time");
            entity.Property(e => e.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
        });

        // ── ProductionOrder ──────────────────────────────────────────────────
        modelBuilder.Entity<ProductionOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("production_order", tb => tb.HasComment("Lệnh sản xuất"));
            entity.HasIndex(e => e.OrderNo, "UX_production_order_orderno").IsUnique();
            entity.HasIndex(e => e.ProductId, "IX_production_order_product_id");
            entity.HasIndex(e => e.LineId, "IX_production_order_line_id");
            entity.HasIndex(e => e.Status, "IX_production_order_status");
            entity.HasIndex(e => e.IsDeleted, "IX_production_order_is_deleted");
            entity.HasIndex(e => e.CreatedDate, "IX_production_order_created_date");

            entity.Property(e => e.OrderNo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PlannedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CompletedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ScrapQuantity).HasPrecision(18, 4);
            entity.Property(e => e.BatchNo).HasMaxLength(100);
            entity.Property(e => e.SalesOrderNo).HasMaxLength(100);
            entity.Property(e => e.Remark).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasMany(e => e.Materials)
                .WithOne()
                .HasForeignKey(m => m.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Operations)
                .WithOne()
                .HasForeignKey(o => o.ProductionOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ProductionOrderMaterial ──────────────────────────────────────────
        modelBuilder.Entity<ProductionOrderMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("production_order_material", tb => tb.HasComment("Chi tiết nguyên vật liệu của Lệnh sản xuất"));
            entity.HasIndex(e => e.ProductionOrderId, "IX_prod_order_mat_order_id");
            entity.HasIndex(e => e.ProductId, "IX_prod_order_mat_product_id");
            entity.HasIndex(e => e.IsDeleted, "IX_prod_order_mat_is_deleted");

            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.WarehouseName).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.NormQuantity).HasPrecision(18, 6);
            entity.Property(e => e.RequiredQuantity).HasPrecision(18, 4);
            entity.Property(e => e.IssuedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CurrentStock).HasPrecision(18, 4);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        // ── ProductionOrderOperation ─────────────────────────────────────────
        modelBuilder.Entity<ProductionOrderOperation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("production_order_operation", tb => tb.HasComment("Danh sách công đoạn của Lệnh sản xuất"));
            entity.HasIndex(e => e.ProductionOrderId, "IX_prod_order_op_order_id");
            entity.HasIndex(e => e.Sequence, "IX_prod_order_op_seq");
            entity.HasIndex(e => e.IsDeleted, "IX_prod_order_op_is_deleted");

            entity.Property(e => e.OperationCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OperationName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.WorkCenterName).HasMaxLength(100);
            entity.Property(e => e.MachineName).HasMaxLength(100);
            entity.Property(e => e.AssignedEmployeeName).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        // ── Global soft-delete filter ────────────────────────────────────────
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var isDeletedProperty = entityType.ClrType.GetProperty("IsDeleted");
            if (isDeletedProperty is null) continue;

            var filter = BuildIsDeletedFilter(entityType.ClrType, isDeletedProperty);
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}
