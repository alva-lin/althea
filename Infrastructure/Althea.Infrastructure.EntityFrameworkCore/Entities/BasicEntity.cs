#pragma warning disable CS8618
namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public abstract class BasicEntity<TKey> : IBasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TKey Id { get; set; }
}

public abstract class BasicEntityWithAudit<TKey> : BasicEntity<TKey>, IBasicEntity<TKey, BasicAudit>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     审计信息
    /// </summary>
    public BasicAudit Audit { get; set; } = BasicAudit.Default;
}
