namespace Althea.Data.Domains.ChatDomain;

/// <summary>
///     聊天操作日志
/// </summary>
public class ChatOperatorLog : BasicEntityWithAudit<long>
{
    /// <summary>
    ///     操作
    /// </summary>
    public ChatOperator Operator { get; set; }

    public Chat Chat { get; set; } = null!;

    public long ChatId { get; set; }

    /// <summary>
    ///     操作的消息
    /// </summary>
    public Message Message { get; set; } = null!;

    public long MessageId { get; set; }

    /// <summary>
    ///     接收的消息（某些操作会收到消息）
    /// </summary>
    public Message? Received { get; set; }

    public long ReceivedId { get; set; }

    public int PromptUsage { get; set; }

    public int CompletionUsage { get; set; }

    public int TotalUsage => PromptUsage + CompletionUsage;
}

public class ChatOperatorLogEntityConfiguration : BasicEntityWithAuditConfiguration<ChatOperatorLog, long, BasicAudit>
{
    public override void Configure(EntityTypeBuilder<ChatOperatorLog> builder)
    {
        base.Configure(builder);

        builder.HasOne(log => log.Chat).WithMany(chat => chat.Logs).IsRequired(false).HasForeignKey(log => log.ChatId);
        builder.HasOne(log => log.Message).WithMany().IsRequired(false).HasForeignKey(log => log.MessageId);
        builder.HasOne(log => log.Received).WithMany().IsRequired(false).HasForeignKey(log => log.ReceivedId);

        Console.WriteLine("ChatEntityConfiguration.Configure");
    }
}
