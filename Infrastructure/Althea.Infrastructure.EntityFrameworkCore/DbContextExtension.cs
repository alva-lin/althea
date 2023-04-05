using System.Reflection;

using Althea.Infrastructure.EntityFrameworkCore.Converters;
using Althea.Infrastructure.EntityFrameworkCore.Entities;

namespace Althea.Infrastructure.EntityFrameworkCore;

public static class DbContextExtension
{
    /// <summary>
    ///     将所有继承自 IBasicEntity 的实体注册为实体配置
    /// </summary>
    /// <param name="modelBuilder"></param>
    /// <param name="assemblies"></param>
    public static ModelBuilder ConfigureBaseEntityTypes(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        var basicEntityType = typeof(IBasicEntity<>);

        assemblies = assemblies.Union(new[] { typeof(IBasicEntity).Assembly }).ToArray();
        var entityTypes = assemblies.SelectMany(assembly =>
                assembly.GetTypes().Where(t =>
                    t is { IsClass: true, IsAbstract: false }
                 && t.IsAssignableTo(typeof(IBasicEntity))))
            .Distinct();

        foreach (var entityType in entityTypes)
        {
            var tKey = entityType.GetInterfaces()
                .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == basicEntityType)
                .GetGenericArguments()[0];

            Type entityConfigurationType;

            if (typeof(DeletableEntity<>).MakeGenericType(tKey).IsAssignableFrom(entityType))
            {
                entityConfigurationType
                    = typeof(DeletableEntityConfiguration<,>).MakeGenericType(entityType, tKey);
            }
            else if (typeof(IAuditable).IsAssignableFrom(entityType))
            {
                var tIAudit = entityType.GetInterface(typeof(IBasicEntity<,>).Name)!.GetGenericArguments()[1];

                entityConfigurationType
                    = typeof(BasicEntityWithAuditConfiguration<,,>).MakeGenericType(entityType, tKey, tIAudit);
            }
            else
            {
                entityConfigurationType = typeof(BasicEntityConfiguration<,>).MakeGenericType(entityType, tKey);
            }

            Console.WriteLine("Configuring: " + entityType.Name);
            var entityConfiguration = Activator.CreateInstance(entityConfigurationType);
            modelBuilder.ApplyConfiguration((dynamic)entityConfiguration!);
        }

        var basicEntityConfigurationTypes = new[]
        {
            typeof(BasicEntityConfiguration<,>),
            typeof(BasicEntityWithAuditConfiguration<,,>),
            typeof(DeletableEntityConfiguration<,>)
        };
        var entityConfigurationTypes = assemblies.SelectMany(assembly =>
                assembly.GetTypes().Where(t =>
                    t is { IsClass: true, IsAbstract: false, IsGenericType: false, BaseType.IsGenericType: true } &&
                    basicEntityConfigurationTypes.Any(t2 => t2 == t.BaseType.GetGenericTypeDefinition()))
            );
        foreach (var entityConfigurationType in entityConfigurationTypes)
        {
            var entityConfiguration = Activator.CreateInstance(entityConfigurationType);
            modelBuilder.ApplyConfiguration((dynamic)entityConfiguration!);
        }

        return modelBuilder;
    }

    /// <summary>
    ///     审计信息保存为 Json 列
    /// </summary>
    public static ModelBuilder ApplyAuditInfo(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType is { IsClass: true } &&
                    property.ClrType.IsAssignableFrom(typeof(IAudit)))
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
