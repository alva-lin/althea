using Althea.Infrastructure.EntityFrameworkCore.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Althea.Infrastructure.EntityFrameworkCore;

public class BasicEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
    where TEntity : BasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).Name);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        // 使用 PostgreSQL 的 jsonb 类型
        builder.Property(e => e.Audit).HasColumnType("jsonb");
    }
}
