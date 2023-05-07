using System.Runtime.CompilerServices;
using System.Text;
using Althea.Core.Services;
using Althea.Infrastructure.DependencyInjection;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;

namespace Althea.Data.Domains.ChatDomain;

public interface IChatService
{
    /// <summary>
    ///     获取聊天列表
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatInfoDto[]> GetChatsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     获取聊天
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatInfoDto> GetChatInfoAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    ///     生成聊天名称
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GenerateTitleAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    ///     重命名聊天
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<bool> RenameChatAsync(long id, string name);

    /// <summary>
    ///     删除聊天
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> DeleteChatAsync(long id);

    /// <summary>
    ///     恢复聊天
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> RestoreChatAsync(long id);

    /// <summary>
    ///     获取消息列表
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MessageDto[]> GetMessagesAsync(long chatId, CancellationToken cancellationToken);

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="chatId"></param>
    /// <param name="prevMessageId"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<ChatResponse> SendMessageAsync(string message,
        long? chatId, long? prevMessageId,
        string? model,
        CancellationToken cancellationToken);
}

[LifeScope(LifeScope.Scope, typeof(IChatService))]
public class ChatService : BasicService, IChatService
{
    private readonly AltheaDbContext _context;

    private readonly IMapper _mapper;

    private readonly IOpenAIService _openAIService;

    private readonly TikTokenService _tikTokenService;

    private readonly IAuthInfoProvider _authInfoProvider;

    public ChatService(ILogger<ChatService> logger, IOpenAIService openAIService, AltheaDbContext context,
        TikTokenService tikTokenService, IMapper mapper, IAuthInfoProvider authInfoProvider)
        : base(logger)
    {
        _openAIService = openAIService;
        _context = context;
        _tikTokenService = tikTokenService;
        _mapper = mapper;
        _authInfoProvider = authInfoProvider;
    }

    #region Chat

    public async Task<ChatInfoDto[]> GetChatsAsync(CancellationToken cancellationToken = default)
    {
        var chats = await _context.Set<Chat>().AsNoTracking().AsQueryable()
            .Where(chat => chat.Own == _authInfoProvider.CurrentUser)
            .OrderByDescending(chat => chat.Id)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return _mapper.Map<ChatInfoDto[]>(chats);
    }

    public async Task<ChatInfoDto> GetChatInfoAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var chat = await FindChatAsync(chatId, true, cancellationToken: cancellationToken);
        return _mapper.Map<ChatInfoDto>(chat);
    }

    public async Task<string> GenerateTitleAsync(long chatId, CancellationToken cancellationToken)
    {
        var chat = await FindChatAsync(chatId, true, cancellationToken: cancellationToken);

        var content = chat.GetChatContext().Aggregate(
            "为下面这段对话归纳出一个准确、简练的标题，不能超过8个字\n",
            (current, message) => current + "\n" + message.Type.ToString("G") + ": " + message.Content
        );

        var messages = new[]
        {
            ChatMessage.FromUser(content)
        };

        var response = await _openAIService.ChatCompletion.CreateCompletion(new()
        {
            Messages = messages
        }, Models.ChatGpt3_5Turbo, cancellationToken);

        if (!response.Successful) return string.Empty;

        var title = response.Choices.First().Message.Content.Trim().Trim('\"').Trim();

        chat.Name = title;
        _context.Update(chat);
        await _context.SaveChangesAsync(cancellationToken);

        return title;
    }

    public async Task<bool> RenameChatAsync(long id, string name)
    {
        var chat = await FindChatAsync(id);
        chat.Name = name;
        _context.Update(chat);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteChatAsync(long id)
    {
        var chat = await FindChatAsync(id);
        _context.Remove(chat);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RestoreChatAsync(long id)
    {
        var chat = await FindChatAsync(id, ignoreGlobalQuery: true);
        chat.Audit.IsDeleted = false;
        _context.Update(chat);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region Message

    public async Task<MessageDto[]> GetMessagesAsync(long chatId, CancellationToken cancellationToken)
    {
        var chat = await FindChatAsync(chatId, true, cancellationToken: cancellationToken);
        return _mapper.Map<MessageDto[]>(chat.Messages);
    }

    public async IAsyncEnumerable<ChatResponse> SendMessageAsync(string message,
        long? chatId, long? prevMessageId,
        string? model,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();

        Chat? chat;
        if (chatId is not null)
        {
            chat = await GetChatAsync(chatId.Value, true, cancellationToken: cancellationToken);
            if (chat is null) throw new($"Chat[{chatId}] not found");
        }
        else
        {
            chat = await CreateChatAsync(model, cancellationToken);
            chatId = chat.Id;
        }

        var sent = chat.AddMessage(_context, _tikTokenService, prevMessageId, message.Trim(), MessageType.User);
        var sentDto = _mapper.Map<MessageDto>(sent);
        yield return new(sentDto, new()
        {
            ChatId = chatId.Value,
            Order = sent.Order + 1,
            Content = string.Empty,
            Type = MessageType.Assistant,
            Usage = 0,
            SendTime = DateTime.UtcNow
        }, false);

        var result = _openAIService.ChatCompletion.CreateCompletionAsStream(new()
        {
            Messages = chat.GetChatMessages(sent.Id)
        }, chat.Model, cancellationToken);

        await foreach (var item in result.WithCancellation(cancellationToken))
        {
            if (item.Successful)
            {
                var response = item.Choices.First();

                // OpenAI 的第一个返回值里，Content 为 null 值，所以做相关处理
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                var content = response.Message.Content ?? string.Empty;
                sb.Append(content);

                var tempReceived = new MessageDto
                {
                    ChatId = chatId.Value,
                    Order = sent.Order + 1,
                    Content = content,
                    Type = MessageType.Assistant,
                    Usage = _tikTokenService.CalculateTokenLength(sb.ToString()),
                    SendTime = DateTime.UtcNow
                };

                yield return new(sentDto, tempReceived, false);
            }
            else
            {
                var errorMessage = item.Error is null ? "Unknown Error" : $"{item.Error.Code}: {item.Error.Message}";
                Logger.Log(LogLevel.Error, "OpenAI GPT3 Error: {ErrorMessage}", errorMessage);
                break;
            }
        }

        var receivedMessage = sb.ToString().Trim();
        var log = chat.AddRequestLog(_context, _tikTokenService, sent.Id, receivedMessage, out var received);
        Logger.LogInformation("Request Log: {Log}", log.GetLogInfo());

        await _context.SaveChangesAsync(CancellationToken.None);

        yield return new(_mapper.Map<MessageDto>(sent), _mapper.Map<MessageDto>(received), true);
    }

    #endregion

    #region Private Methods

    private async Task<Chat> CreateChatAsync(string? model = null, CancellationToken cancellationToken = default)
    {
        if (model != Models.ChatGpt3_5Turbo || model != Models.Gpt_4) model = Models.ChatGpt3_5Turbo;

        var chat = new Chat
        {
            Name = "Default Chat",
            Model = model,
            Own = _authInfoProvider.CurrentUser
        };
        await _context.Set<Chat>().AddAsync(chat, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return chat;
    }

    private async Task<Chat> FindChatAsync(long id,
        bool includeMessage = false, bool includeLog = false,
        bool ignoreGlobalQuery = false,
        CancellationToken cancellationToken = default)
    {
        return await GetChatAsync(id, includeMessage, includeLog, ignoreGlobalQuery, cancellationToken)
            ?? throw new("Chat not found");
    }

    private Task<Chat?> GetChatAsync(long id,
        bool includeMessage = false, bool includeLog = false,
        bool ignoreGlobalQuery = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<Chat>().AsQueryable()
            .IncludeIf(includeMessage, chat => chat.Messages)
            .IncludeIf(includeLog, chat => chat.Logs)
            .AsSingleQuery();
        if (ignoreGlobalQuery)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.FirstOrDefaultAsync(chat => chat.Id == id, cancellationToken);
    }

    #endregion
}
