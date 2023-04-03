using Althea.Core.Services;

using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Althea.Data.Domains.ChatDomain;

public class Chat : DeletableEntity<long>
{
    /// <summary>
    /// 聊天名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 使用的模型
    /// </summary>
    public string Model { get; set; } = null!;

    /// <summary>
    /// 当前消息的 token 数
    /// </summary>
    public int CurrentUsage { get; protected set; }

    /// <summary>
    /// 累计使用的 token 总额
    /// </summary>
    public int TotalUsage { get; protected set; }

    /// <summary>
    /// 最后一次发送消息的时间
    /// </summary>
    public DateTime? LastSendTime { get; protected set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public ICollection<ChatOperatorLog> Logs { get; set; } = new List<ChatOperatorLog>();

    /// <summary>
    /// 文本总长度
    /// </summary>
    public int TotalLength => Messages.Sum(message => message.Content.Length);

    /// <summary>
    /// 调用 API 时，发送的消息
    /// </summary>
    [NotMapped]
    public List<ChatMessage> ChatMessages => Messages.Select(GetChatMessage).ToList();

    public Message AddMessage(AltheaDbContext dbContext, TikTokenService tikTokenService, string content,
        MessageType type)
    {
        LastSendTime = DateTime.UtcNow;

        var message = new Message
        {
            Chat    = this,
            Order   = Messages.Count + 1,
            Type    = type,
            Content = content,
            Usage   = tikTokenService.CalculateTokenLength(content, Model)
        };

        Messages.Add(message);
        dbContext.Add(message);
        dbContext.Update(this);

        return message;
    }

    public ChatOperatorLog AddSendLog(AltheaDbContext dbContext, TikTokenService tikTokenService, Message sent,
        string receivedMessage, out Message received)
    {
        // 添加接收到的消息
        received = AddMessage(dbContext, tikTokenService, receivedMessage, MessageType.Assistant);

        // 计算 prompt 的 token 数
        // API 实际计算时，每条消息的前面都会加上 System/User/Assistant，所以这里加上 5
        var usage = Messages.Sum(message => message.Usage + 5);

        // 添加操作日志
        ChatOperatorLog log = new ChatOperatorLog()
        {
            Operator        = ChatOperator.Send,
            Chat            = this,
            Message         = sent,
            Received        = received,
            PromptUsage     = usage - received.Usage,
            CompletionUsage = received.Usage,
        };
        dbContext.Add(log);
        Logs.Add(log);

        // 更新当前 token 数
        CurrentUsage =  usage;
        TotalUsage   += usage;

        dbContext.Update(this);

        return log;
    }

    private static ChatMessage GetChatMessage(Message message)
    {
        Func<string, ChatMessage> factory = message.Type switch
        {
            MessageType.User      => ChatMessage.FromUser,
            MessageType.Assistant => ChatMessage.FromAssistant,
            MessageType.System    => ChatMessage.FromSystem,
            _                     => throw new ArgumentOutOfRangeException()
        };
        return factory(message.Content);
    }
}

public class ChatEntityConfiguration : DeletableEntityConfiguration<Chat, long>
{
    public override void Configure(EntityTypeBuilder<Chat> builder)
    {
        base.Configure(builder);

        builder.HasMany(chat => chat.Logs).WithOne(log => log.Chat).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        Console.WriteLine("ChatEntityConfiguration.Configure");
    }
}
