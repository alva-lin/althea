#pragma warning disable CS8618
namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public abstract class EditableAuditEntity<TKey> : BasicEntity<TKey>, IAuditable<EditableAudit>
    where TKey : IEquatable<TKey>
{
    public new EditableAudit Audit { get; set; }
}
