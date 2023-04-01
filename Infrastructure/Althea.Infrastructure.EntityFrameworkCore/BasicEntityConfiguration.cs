﻿using Althea.Infrastructure.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Althea.Infrastructure.EntityFrameworkCore;

/// <summary>
/// 基础实体配置
/// </summary>
/// <remarks>这是没有审计信息的实体配置，如果实体带有审计信息，请继承 <see cref="BasicEntityWithAuditConfiguration{TEntity,TKey,TAudit}"/></remarks>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
public class BasicEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}

/// <summary>
/// 带有审计信息的实体配置
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TKey">主键类型</typeparam>
/// <typeparam name="TAudit">审计信息类型</typeparam>
public class BasicEntityWithAuditConfiguration<TEntity, TKey, TAudit> : BasicEntityConfiguration<TEntity, TKey>
    where TEntity : BasicEntity<TKey>, IAuditable<TAudit>
    where TKey : IEquatable<TKey>
    where TAudit : IAudit
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // 使用 PostgreSQL 的 jsonb 类型
        builder.Property(e => e.Audit).HasColumnType("jsonb");
    }
}