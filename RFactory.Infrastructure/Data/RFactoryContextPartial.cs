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
