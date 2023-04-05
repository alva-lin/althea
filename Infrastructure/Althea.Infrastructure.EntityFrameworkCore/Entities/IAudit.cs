using System.Text.Json.Serialization;

namespace Althea.Infrastructure.EntityFrameworkCore.Entities;

/// <summary>
///     审计信息
/// </summary>
public interface IAudit
{
}

/// <summary>
///     基础审计信息，记录创建信息
/// </summary>
[Owned]
public class BasicAudit : IAudit
{
    public static readonly BasicAudit Default = new()
    {
        CreatedBy    = string.Empty,
        CreationTime = DateTime.UtcNow
    };

    /// <summary>
    ///     创建人
    /// </summary>
    [JsonPropertyOrder(11)]
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    ///     创建时间
    /// </summary>
    [JsonPropertyOrder(10)]
    public DateTime CreationTime { get; set; }
}

/// <summary>
///     记录编辑信息
/// </summary>
public class EditableAudit : BasicAudit
{
    public new static readonly EditableAudit Default = new()
    {
        CreatedBy    = string.Empty,
        CreationTime = DateTime.UtcNow
    };

    /// <summary>
    ///     修改人
    /// </summary>
    [JsonPropertyOrder(21)]
    public string? ModifiedBy { get; set; }

    /// <summary>
    ///     修改时间
    /// </summary>

    [JsonPropertyOrder(20)]
    public DateTime? ModifiedTime { get; set; }
}

/// <summary>
///     记录删除信息
/// </summary>
public class DeletableAudit : EditableAudit
{
    public new static readonly DeletableAudit Default = new()
    {
        CreatedBy    = string.Empty,
        CreationTime = DateTime.UtcNow
    };

    /// <summary>
    ///     是否删除
    /// </summary>

    [JsonPropertyOrder(1)]
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     删除人
    /// </summary>

    [JsonPropertyOrder(32)]
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     删除时间
    /// </summary>
    [JsonPropertyOrder(31)]
    public DateTime? DeletedTime { get; set; }
}
