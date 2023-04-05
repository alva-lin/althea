namespace Althea.Data.Domains.ChatDomain;

/// <summary>
///     对话消息
/// </summary>
public class Message : DeletableEntity<long>
{
    public Chat Chat { get; set; } = null!;

    public long ChatId { get; set; }

    /// <summary>
    ///     序号
    /// </summary>
    public long Order { get; set; }

    /// <summary>
    ///     消息类型
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    ///     消息内容
    /// </summary>
    public string Content { get; internal set; } = null!;

    /// <summary>
    ///     消息使用的 token 数
    /// </summary>
    public int Usage { get; internal set; }
}
