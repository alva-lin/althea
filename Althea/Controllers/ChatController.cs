using System.Runtime.CompilerServices;
using Althea.Data.Domains.ChatDomain;
using Microsoft.AspNetCore.Mvc;

namespace Althea.Controllers;

/// <summary>
///     聊天记录控制器
/// </summary>
public class ChatController : BasicApiController
{
    private readonly IChatService _chatService;

    public ChatController(ILogger<ChatController> logger, IChatService chatService)
        : base(logger)
    {
        _chatService = chatService;
    }

    /// <summary>
    ///     获取聊天列表
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResponseResult<ChatInfoDto[]>> GetChatListAsync(CancellationToken cancellationToken = default)
    {
        return await _chatService.GetChatsAsync(false, false, cancellationToken);
    }

    /// <summary>
    ///     新建聊天
    /// </summary>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<ResponseResult<ChatInfoDto>> CreateChatAsync()
    {
        return await _chatService.CreateChatAsync();
    }

    /// <summary>
    ///     重命名聊天
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpPost("{id:long}/rename")]
    public async Task<ResponseResult<bool>> RenameChatAsync(long id, string name)
    {
        return await _chatService.RenameChatAsync(id, name);
    }

    /// <summary>
    ///     删除聊天
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id}/delete")]
    public async Task<ResponseResult<bool>> DeleteChatAsync(long id)
    {
        return await _chatService.DeleteChatAsync(id);
    }

    /// <summary>
    ///     恢复聊天
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id}/restore")]
    public async Task<ResponseResult<bool>> RestoreChatAsync(long id)
    {
        return await _chatService.RestoreChatAsync(id);
    }

    /// <summary>
    ///     获取聊天记录
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{chatId}/messages")]
    public async Task<ResponseListResult<MessageDto>> GetMessagesAsync(long chatId,
        CancellationToken cancellationToken = default)
    {
        return await _chatService.GetMessagesAsync(chatId, cancellationToken);
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="chatId">聊天Id</param>
    /// <param name="lastMessageId">最后一条消息的id</param>
    /// <param name="message">发送内容</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{chatId}/send")]
    public async IAsyncEnumerable<ResponseResult<ChatResponse>> SendMessage(
        long chatId, long lastMessageId, string message,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var received in _chatService.SendMessageAsync(chatId, lastMessageId, message, cancellationToken))
            yield return received;
    }
}
