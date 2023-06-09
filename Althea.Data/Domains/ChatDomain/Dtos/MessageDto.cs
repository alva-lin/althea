﻿namespace Althea.Data.Domains.ChatDomain;

public class MessageDto
{
    public long Id { get; set; }

    public long ChatId { get; set; }

    public Guid? VoiceId { get; set; }

    public long? PrevMessageId { get; set; }

    public long[] NextMessageIds { get; set; } = Array.Empty<long>();

    public bool IsRoot => PrevMessageId is null;

    /// <summary>
    ///     序号
    /// </summary>
    public long Order { get; set; }

    /// <summary>
    ///     消息主题
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    ///     消息类型
    /// </summary>
    public MessageType Type { get; set; }

    /// <summary>
    ///     Token 数
    /// </summary>
    public long Usage { get; set; }

    /// <summary>
    ///     发送时间
    /// </summary>
    public DateTime SendTime { get; set; }
}
