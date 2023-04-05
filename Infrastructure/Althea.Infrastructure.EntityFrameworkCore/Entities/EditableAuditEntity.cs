#pragma warning disable CS8618
namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public abstract class EditableAuditEntity<TKey> : BasicEntity<TKey>,
    IBasicEntity<TKey, EditableAudit>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     审计信息
    /// </summary>
    public EditableAudit Audit { get; set; } = EditableAudit.Default;
}
