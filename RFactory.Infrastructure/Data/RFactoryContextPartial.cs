using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RFactory.Infrastructure.Entities;

namespace RFactory.Infrastructure.Data;

/// <summary>
/// Extends the auto-generated <see cref="RFactoryContext"/> with DbSets for entities
/// that were added manually (not scaffolded from an existing table).
/// </summary>
public partial class RFactoryContext
{
    public virtual DbSet<ProductionOrder> ProductionOrders { get; set; }

    public virtual DbSet<ProductionOrderMaterial> ProductionOrderMaterials { get; set; }

    public virtual DbSet<ProductionOrderOperation> ProductionOrderOperations { get; set; }

    public virtual DbSet<MasterProductionPlan> MasterProductionPlans { get; set; }

    public virtual DbSet<MasterProductionPlanItem> MasterProductionPlanItems { get; set; }

    public virtual DbSet<DetailedProductionPlan> DetailedProductionPlans { get; set; }

    public virtual DbSet<DetailedProductionPlanItem> DetailedProductionPlanItems { get; set; }

    public virtual DbSet<DetailedProductionPlanSchedule> DetailedProductionPlanSchedules { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
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

        // ── MasterProductionPlan ─────────────────────────────────────────────
        modelBuilder.Entity<MasterProductionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("master_production_plan", tb => tb.HasComment("Kế hoạch sản xuất tổng thể"));
            entity.HasIndex(e => e.PlanCode, "UX_master_prod_plan_code").IsUnique();
            entity.HasIndex(e => e.Status, "IX_master_prod_plan_status");
            entity.HasIndex(e => e.IsDeleted, "IX_master_prod_plan_is_deleted");

            entity.Property(e => e.PlanCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PlanName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PeriodType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PeriodName).HasMaxLength(50);
            entity.Property(e => e.WorkshopName).HasMaxLength(100);
            entity.Property(e => e.TotalQuantity).HasPrecision(18, 4);
            entity.Property(e => e.Remark).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(i => i.MasterPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── MasterProductionPlanItem ─────────────────────────────────────────
        modelBuilder.Entity<MasterProductionPlanItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("master_production_plan_item", tb => tb.HasComment("Chi tiết sản phẩm kế hoạch tổng thể"));
            entity.HasIndex(e => e.MasterPlanId, "IX_master_item_plan_id");
            entity.HasIndex(e => e.ProductId, "IX_master_item_product_id");
            entity.HasIndex(e => e.IsDeleted, "IX_master_item_is_deleted");

            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.PlannedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.AllocatedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.RemainingQuantity).HasPrecision(18, 4);
            entity.Property(e => e.DailyCapacity).HasPrecision(18, 4);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        // ── DetailedProductionPlan ───────────────────────────────────────────
        modelBuilder.Entity<DetailedProductionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("detailed_production_plan", tb => tb.HasComment("Kế hoạch sản xuất chi tiết"));
            entity.HasIndex(e => e.PlanCode, "UX_detailed_prod_plan_code").IsUnique();
            entity.HasIndex(e => e.MasterPlanId, "IX_detailed_prod_plan_master_id");
            entity.HasIndex(e => e.Status, "IX_detailed_prod_plan_status");
            entity.HasIndex(e => e.FromDate, "IX_detailed_prod_plan_from_date");
            entity.HasIndex(e => e.ToDate, "IX_detailed_prod_plan_to_date");
            entity.HasIndex(e => e.IsDeleted, "IX_detailed_prod_plan_is_deleted");

            entity.Property(e => e.PlanCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PlanName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MasterPlanCode).HasMaxLength(50);
            entity.Property(e => e.WorkshopName).HasMaxLength(100);
            entity.Property(e => e.LineName).HasMaxLength(100);
            entity.Property(e => e.DepartmentName).HasMaxLength(100);
            entity.Property(e => e.PeriodType).IsRequired().HasMaxLength(20);
            entity.Property(e => e.AllocationMethod).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TotalRequiredQuantity).HasPrecision(18, 4);
            entity.Property(e => e.TotalPlannedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.Remark).HasColumnType("text");
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);

            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(i => i.PlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Schedules)
                .WithOne()
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── DetailedProductionPlanItem ───────────────────────────────────────
        modelBuilder.Entity<DetailedProductionPlanItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("detailed_production_plan_item", tb => tb.HasComment("Nhu cầu sản phẩm kế hoạch chi tiết"));
            entity.HasIndex(e => e.PlanId, "IX_det_plan_item_plan_id");
            entity.HasIndex(e => e.ProductId, "IX_det_plan_item_product_id");
            entity.HasIndex(e => e.IsDeleted, "IX_det_plan_item_is_deleted");

            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UnitName).HasMaxLength(50);
            entity.Property(e => e.RequiredQuantity).HasPrecision(18, 4);
            entity.Property(e => e.CurrentStock).HasPrecision(18, 4);
            entity.Property(e => e.PlannedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.DailyCapacity).HasPrecision(18, 4);
            entity.Property(e => e.SalesOrderNo).HasMaxLength(100);
            entity.Property(e => e.Remark).HasMaxLength(255);
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);
            entity.Property(e => e.UpdatedBy).HasMaxLength(50);
        });

        // ── DetailedProductionPlanSchedule ───────────────────────────────────
        modelBuilder.Entity<DetailedProductionPlanSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("detailed_production_plan_schedule", tb => tb.HasComment("Lịch phân bổ sản xuất theo ngày/ca"));
            entity.HasIndex(e => e.PlanId, "IX_det_plan_sch_plan_id");
            entity.HasIndex(e => e.PlanItemId, "IX_det_plan_sch_item_id");
            entity.HasIndex(e => e.ScheduleDate, "IX_det_plan_sch_date");
            entity.HasIndex(e => e.ProductionOrderId, "IX_det_plan_sch_prod_order");
            entity.HasIndex(e => e.IsDeleted, "IX_det_plan_sch_is_deleted");

            entity.Property(e => e.ShiftName).HasMaxLength(50);
            entity.Property(e => e.Capacity).HasPrecision(18, 4);
            entity.Property(e => e.PlannedQuantity).HasPrecision(18, 4);
            entity.Property(e => e.ProductionOrderNo).HasMaxLength(50);
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

    private static LambdaExpression BuildIsDeletedFilter(Type clrType, PropertyInfo isDeletedProperty)
    {
        var parameter = Expression.Parameter(clrType, "e");
        var property = Expression.Property(parameter, isDeletedProperty);

        // IsDeleted is bool? on most scaffolded entities, bool on TableBase; handle both.
        Expression notDeleted = isDeletedProperty.PropertyType == typeof(bool)
            ? Expression.Equal(property, Expression.Constant(false))
            : Expression.NotEqual(property, Expression.Constant(true, typeof(bool?)));

        return Expression.Lambda(notDeleted, parameter);
    }
}
