namespace Althea.Data.Domains.ChatDomain;

/// <summary>
///     对话消息
/// </summary>
public class Message : DeletableEntity<long>
{
    /// <summary>
    ///     聊天
    /// </summary>
    public Chat Chat { get; set; } = null!;

    /// <summary>
    ///     上一条消息
    /// </summary>
    public Message? PrevMessage { get; set; }

    /// <summary>
    ///     下一条消息
    /// </summary>
    public List<Message> NextMessages { get; set; } = new();

    /// <summary>
    ///     日志，作为 prompt 时
    /// </summary>
    public List<RequestLog> PromptsLog { get; set; } = new();

    /// <summary>
    ///     日志，作为 completion 时
    /// </summary>
    public RequestLog? CompletionLog { get; set; }

    /// <summary>
    ///     是否是第一条消息
    /// </summary>
    public bool IsRoot => PrevMessage is null;

    /// <summary>
    ///     序号
    /// </summary>
    public long Order { get; internal set; }

    /// <summary>
    ///     消息类型
    /// </summary>
    public MessageType Type { get; init; }

    /// <summary>
    ///     消息内容
    /// </summary>
    public string Content { get; internal init; } = null!;

    /// <summary>
    ///     消息使用的 token 数
    /// </summary>
    public int Usage { get; internal init; }
}

public class MessageEntityConfiguration : DeletableEntityConfiguration<Message, long>
{
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
        base.Configure(builder);

        builder
            .HasMany(message => message.NextMessages)
            .WithOne(message => message.PrevMessage)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        Console.WriteLine("Message.Configure");
    }
}