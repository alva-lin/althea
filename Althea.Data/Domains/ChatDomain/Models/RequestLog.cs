using System.Text;

namespace Althea.Data.Domains.ChatDomain;

/// <summary>
///     发送日志
/// </summary>
public class RequestLog : BasicEntityWithAudit<long>
{
    public Chat Chat { get; set; } = null!;

    public long ChatId { get; set; }

    /// <summary>
    ///     发送的消息
    /// </summary>
    public Message[] Prompts { get; set; } = Array.Empty<Message>();

    public long[] PromptIds => Prompts.Select(m => m.Id).ToArray();

    /// <summary>
    ///     接收的消息（某些操作会收到消息）
    /// </summary>
    public Message Completion { get; set; } = null!;

    public long CompletionId { get; set; }

    public int PromptUsage { get; set; }

    public int CompletionUsage { get; set; }

    public int TotalUsage => PromptUsage + CompletionUsage;

    /// <summary>
    ///     发送成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     错误消息
    /// </summary>
    public string? ErrorInfo { get; set; }

    public string GetLogInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Chat: {Chat.Name} ({ChatId}) {Chat.Model} {Chat.Own}");
        sb.AppendLine($"CreateTime: {Audit.CreationTime}");
        sb.AppendLine($"PromptIds: {string.Join(", ", PromptIds)}");
        sb.AppendLine($"CompletionId: {Completion.Id}");

        sb.Append($"PromptUsage: {PromptUsage}")
            .Append($"CompletionUsage: {CompletionUsage}")
            .Append($"TotalUsage: {TotalUsage}")
            .AppendLine();
        if (!Success) sb.AppendLine($"ErrorInfo: {ErrorInfo}");

        return sb.ToString().Trim();
    }
}

public class RequestLogEntityConfiguration : BasicEntityWithAuditConfiguration<RequestLog, long, BasicAudit>
{
    public override void Configure(EntityTypeBuilder<RequestLog> builder)
    {
        base.Configure(builder);

        builder.HasOne(log => log.Chat).WithMany(chat => chat.Logs);
        builder.HasOne(log => log.Completion)
            .WithOne(message => message.CompletionLog)
            .HasForeignKey<RequestLog>(log => log.CompletionId)
            .IsRequired(false);
        builder.HasMany(log => log.Prompts).WithMany(message => message.PromptsLog);

        Console.WriteLine("RequestLog.Configure");
    }
}
