using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RFactory.Infrastructure.Entities;

namespace RFactory.Infrastructure.Data;

/// <summary>
/// Applies a global query filter so that every entity with an <c>IsDeleted</c> property
/// automatically excludes soft-deleted rows (IsDeleted == true) from all reads.
/// Use <see cref="Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.IgnoreQueryFilters{TEntity}"/>
/// on a query when soft-deleted rows are needed (e.g. an "include deleted" admin view).
/// </summary>
public partial class RFactoryContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
        });


        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var isDeletedProperty = entityType.ClrType.GetProperty("IsDeleted");
            if (isDeletedProperty is null)
            {
                continue;
            }

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
