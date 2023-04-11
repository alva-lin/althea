using System.Runtime.CompilerServices;
using System.Text;
using Althea.Core.Services;
using Althea.Infrastructure.DependencyInjection;
using Althea.Infrastructure.Extensions;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels;

namespace Althea.Data.Domains.ChatDomain;

public interface IChatService
{
    /// <summary>
    ///     获取聊天列表
    /// </summary>
    /// <param name="includeMessage"></param>
    /// <param name="includeLog"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ChatInfoDto[]> GetChatsAsync(bool includeMessage, bool includeLog, CancellationToken cancellationToken);

    /// <summary>
    ///     新建聊天
    /// </summary>
    /// <returns></returns>
    Task<ChatInfoDto> CreateChatAsync();

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
    ///     添加聊天机器人的设定
    /// </summary>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> AddSystemMessageAsync(long id, string message, CancellationToken cancellationToken);

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<ChatResponse> SendMessageAsync(long id, string message,
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

    public async Task<ChatInfoDto[]> GetChatsAsync(bool includeMessage = false, bool includeLog = false,
        CancellationToken cancellationToken = default)
    {
        var chats = await _context.Set<Chat>().AsNoTracking().AsQueryable()
            .Where(chat => chat.Own == _authInfoProvider.CurrentUser)
            .IncludeIf(includeMessage, chat => chat.Messages)
            .IncludeIf(includeLog, chat => chat.Logs)
            .OrderByDescending(chat => chat.Id)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return _mapper.Map<ChatInfoDto[]>(chats);
    }

    public async Task<ChatInfoDto> CreateChatAsync()
    {
        var chat = new Chat
        {
            Name = "Default Chat",
            Model = Models.ChatGpt3_5Turbo,
            Own = _authInfoProvider.CurrentUser
        };
        await _context.Set<Chat>().AddAsync(chat);
        await _context.SaveChangesAsync();
        return _mapper.Map<ChatInfoDto>(chat);
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

    public async Task<MessageDto[]> GetMessagesAsync(long chatId, CancellationToken cancellationToken)
    {
        var chat = await FindChatAsync(chatId, cancellationToken, true);
        return _mapper.Map<MessageDto[]>(chat.Messages);
    }

    public async Task<bool> AddSystemMessageAsync(long id, string message, CancellationToken cancellationToken)
    {
        var chat = await FindChatAsync(id, cancellationToken, true);
        chat.AddMessage(_context, _tikTokenService, message, MessageType.System);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async IAsyncEnumerable<ChatResponse> SendMessageAsync(long id, string message,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        var chat = await FindChatAsync(id, cancellationToken, true);

        message = message.Trim();
        var sent = chat.AddMessage(_context, _tikTokenService, message, MessageType.User);
        var sentDto = _mapper.Map<MessageDto>(sent);
        Logger.Log(LogLevel.Debug, "Chat Message Sent: {Sent}", sent.ToJson(0));

        var result = _openAIService.ChatCompletion.CreateCompletionAsStream(new()
        {
            Messages = chat.ChatMessages
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
                    ChatId = id,
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
        chat.AddSendLog(_context, _tikTokenService, sent, receivedMessage, out var received);
        Logger.Log(LogLevel.Debug, "Chat Message Received: {Received}", received.ToJson(0));

        await _context.SaveChangesAsync(CancellationToken.None);

        yield return new(_mapper.Map<MessageDto>(sent), _mapper.Map<MessageDto>(received), true);
    }

    private async Task<Chat> FindChatAsync(long id,
        CancellationToken cancellationToken = default, bool includeMessage = false, bool includeLog = false,
        bool ignoreGlobalQuery = false)
    {
        return await GetChatAsync(id, cancellationToken, includeMessage, includeLog, ignoreGlobalQuery)
               ?? throw new("Chat not found");
    }

    private Task<Chat?> GetChatAsync(long id,
        CancellationToken cancellationToken = default, bool includeMessage = false, bool includeLog = false,
        bool ignoreGlobalQuery = false)
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
}
