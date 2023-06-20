using Althea.Core.Services;
using OpenAI.ObjectModels.RequestModels;

namespace Althea.Data.Domains.ChatDomain;

public class Chat : DeletableEntity<long>
{
    /// <summary>
    ///     创建人
    /// </summary>
    [StringLength(12)]
    public string Own { get; set; } = null!;

    /// <summary>
    ///     聊天名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    ///     使用的模型
    /// </summary>
    public string Model { get; set; } = null!;

    /// <summary>
    ///     累计使用的 token 总额
    /// </summary>
    public int TotalUsage { get; protected set; }

    /// <summary>
    ///     最后一次发送消息的时间
    /// </summary>
    public DateTime? LastSendTime { get; protected set; }

    /// <summary>
    ///     消息列表
    /// </summary>
    public List<Message> Messages { get; set; } = new();

    /// <summary>
    ///     请求日志列表
    /// </summary>
    public List<RequestLog> Logs { get; set; } = new();

    private Message? GetMessage(long? messageId = null)
    {
        return messageId is null
            ? Messages.Last()
            : Messages.FirstOrDefault(message => message.Id == messageId);
    }

    /// <summary>
    ///     根据最后一条消息 ID 获取消息上下文
    /// </summary>
    /// <param name="lastMessageId">最后一条消息的 ID</param>
    /// <returns></returns>
    public IEnumerable<Message> GetChatContext(long? lastMessageId = null)
    {
        var lastMessage = GetMessage(lastMessageId);
        if (lastMessage is null) return Array.Empty<Message>();

        var messages = new List<Message>();
        var message = lastMessage;
        while (message is not null)
        {
            messages.Add(message);
            message = message.PrevMessage;
        }

        messages.Reverse();

        return messages;
    }

    public ChatMessage[] GetChatMessages(long lastMessageId)
    {
        return GetChatContext(lastMessageId).Select(GetChatMessage).ToArray();
    }

    public Message AddMessage(
        AltheaDbContext dbContext, TikTokenService tikTokenService,
        long? lastMessageId,
        string content, MessageType type)
    {
        LastSendTime = DateTime.UtcNow;

        Message? prevMessage = null;
        if (lastMessageId is not null)
        {
            prevMessage = GetMessage(lastMessageId.Value);
            if (prevMessage is null) throw new ArgumentException($"last message not found: {lastMessageId.Value}");
        }

        var message = new Message
        {
            Chat = this,
            Order = Messages.Count,
            Type = type,
            Content = content,
            Usage = tikTokenService.CalculateTokenLength(content),
            PrevMessage = prevMessage
        };
        prevMessage?.NextMessages.Add(message);

        Messages.Add(message);
        dbContext.Add(message);
        dbContext.Update(this);

        return message;
    }

    public RequestLog AddRequestLog(
        AltheaDbContext dbContext, TikTokenService tikTokenService,
        long sendMessageId,
        string receivedMessage,
        out Message received)
    {
        // 添加接收到的消息
        received = AddMessage(dbContext, tikTokenService, sendMessageId, receivedMessage, MessageType.Assistant);

        // 计算 prompt 的 token 数
        // API 实际计算时，每条消息的前面都会加上 System/User/Assistant，所以这里加上 5
        var usage = Messages.Sum(message => message.Usage + 5);

        // 添加操作日志
        var log = new RequestLog
        {
            Success = true,
            Chat = this,
            Prompts = Messages.ToArray()[..^1].ToList(),
            PromptUsage = usage - received.Usage,
            Completion = received,
            CompletionUsage = received.Usage
        };
        dbContext.Add(log);
        Logs.Add(log);

        // 更新当前 token 数
        TotalUsage += usage;

        dbContext.Update(this);

        return log;
    }

    private static ChatMessage GetChatMessage(Message message)
    {
        Func<string, string?, ChatMessage> factory = message.Type switch
        {
            MessageType.User => ChatMessage.FromUser,
            MessageType.Assistant => ChatMessage.FromAssistant,
            MessageType.System => ChatMessage.FromSystem,
            _ => throw new ArgumentOutOfRangeException()
        };
        return factory(message.Content, null);
    }
}

public class ChatEntityConfiguration : DeletableEntityConfiguration<Chat, long>
{
    public override void Configure(EntityTypeBuilder<Chat> builder)
    {
        base.Configure(builder);

        builder.HasMany(chat => chat.Logs).WithOne(log => log.Chat).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        Console.WriteLine("Chat.Configure");
    }
}
