namespace Althea.Models;

public class SendMessageRequestDto
{
    /// <summary>
    ///     聊天ID
    /// </summary>
    public long? ChatId { get; set; }

    /// <summary>
    ///     最后一条消息ID
    /// </summary>
    public long? PrevMessageId { get; set; }

    /// <summary>
    ///     聊天模型
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    ///     消息
    /// </summary>
    public string Message { get; set; } = null!;
}
