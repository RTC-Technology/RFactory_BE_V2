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
