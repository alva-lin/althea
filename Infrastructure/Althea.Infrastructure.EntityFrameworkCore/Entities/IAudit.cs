namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

public interface IAudit
{
}

[Owned]
public class BasicAudit : IAudit
{
    /// <summary>
    ///     创建人
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    ///     创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }
}

public class EditableAudit : BasicAudit
{
    /// <summary>
    ///     修改人
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    ///     修改时间
    /// </summary>
    public DateTime? ModifiedTime { get; set; }
}

public class DeletableAudit : EditableAudit
{
    /// <summary>
    ///     是否删除
    /// </summary>
    public bool IsDelete { get; set; }

    /// <summary>
    ///     删除人
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     删除时间
    /// </summary>
    public DateTime? DeletedTime { get; set; }
}
