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
