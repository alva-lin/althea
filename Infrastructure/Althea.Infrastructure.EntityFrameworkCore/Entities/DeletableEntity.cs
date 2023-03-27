#pragma warning disable CS8618
namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public abstract class DeletableEntity<TKey> : BasicEntity<TKey>, IAuditable<DeletableAudit>
    where TKey : IEquatable<TKey>
{
    public new DeletableAudit Audit { get; set; }
}
