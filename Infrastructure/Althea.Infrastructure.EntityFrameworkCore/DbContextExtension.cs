using Althea.Infrastructure.EntityFrameworkCore.Converters;
using Althea.Infrastructure.EntityFrameworkCore.Entities;

namespace Althea.Infrastructure.EntityFrameworkCore;

public static class DbContextExtension
{
    /// <summary>
    ///     审计信息保存为 Json 列
    /// </summary>
    public static ModelBuilder ApplyAuditInfo(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsAssignableFrom(typeof(IAudit)))
                {
                    builder.Entity(entityType.ClrType).OwnsOne(property.ClrType, "Audit", navigationBuilder =>
                    {
                        navigationBuilder.ToJson();
                    });
                }
            }
        }
        return builder;
    }

    /// <summary>
    ///     时间保存时，需要转换为 UTC 时间
    /// </summary>
    public static ModelBuilder ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(typeof(DateTimeToUtcConverter));
                }
            }
        }
        return builder;
    }

    public static ModelBuilder AddDateOnlySupport(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateOnly) || property.ClrType == typeof(DateOnly?))
                {
                    property.SetValueConverter(typeof(DateOnlyConverter));
                    property.SetValueComparer(typeof(DateOnlyComparer));
                }
            }
        }
        return builder;
    }

    public static ModelBuilder AddTimeOnlySupport(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(TimeOnly) || property.ClrType == typeof(TimeOnly?))
                {
                    property.SetValueConverter(typeof(TimeOnlyConverter));
                    property.SetValueComparer(typeof(TimeOnlyComparer));
                }
            }
        }
        return builder;
    }
}
