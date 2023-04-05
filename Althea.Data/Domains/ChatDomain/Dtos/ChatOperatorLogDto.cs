namespace Althea.Data.Domains.ChatDomain;

public class ChatOperatorLogDto
{
    public long Id { get; set; }

    public long ChatId { get; set; }

    public long MessageId { get; set; }

    public long? ReceivedId { get; set; }

    /// <summary>
    ///     操作类型
    /// </summary>
    public ChatOperator Operator { get; set; }

    /// <summary>
    ///     本次操作的 Prompt 使用量
    /// </summary>
    public int PromptUsage { get; set; }

    /// <summary>
    ///     本次操作的 Completion 使用量
    /// </summary>
    public int CompletionUsage { get; set; }

    /// <summary>
    ///     本次操作的总使用量
    /// </summary>
    public int TotalUsage { get; set; }

    /// <summary>
    ///     创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }
}
