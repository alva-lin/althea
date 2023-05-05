namespace Althea.Data.Domains.ChatDomain;

public class ChatInfoDto
{
    public long Id { get; set; }

    /// <summary>
    ///     聊天名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     模型
    /// </summary>
    public string Model { get; set; } = null!;

    /// <summary>
    ///     最后一次发送消息的时间
    /// </summary>
    public DateTime? LastSendTime { get; set; }

    /// <summary>
    ///     消息列表
    /// </summary>
    public MessageDto[] Messages { get; set; } = Array.Empty<MessageDto>();
}
