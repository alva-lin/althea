#pragma warning disable CS8618
namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public abstract class DeletableEntity<TKey> : BasicEntity<TKey>, IBasicEntity<TKey, DeletableAudit>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// 审计信息
    /// </summary>
    public DeletableAudit Audit { get; set; } = DeletableAudit.Default;
}
