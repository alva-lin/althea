using Althea.Data.Domains.ChatDomain;
using Althea.Infrastructure;
using Althea.Infrastructure.AspNetCore;
using Althea.Models;

using Microsoft.AspNetCore.Mvc;

namespace Althea.Controllers;

/// <summary>
/// 聊天记录控制器
/// </summary>
public class ChatController : BasicApiController
{
    public ChatController(ILogger<ChatController> logger, IChatService chatService)
        : base(logger)
    {
        _chatService = chatService;

    }

    private readonly IChatService _chatService;

    /// <summary>
    /// 获取聊天列表
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ResponseListResult<ChatInfoDto>> GetChatListAsync(CancellationToken cancellationToken = default)
    {
        return await _chatService.GetChatsAsync(false, false, cancellationToken);
    }

    /// <summary>
    /// 新建聊天
    /// </summary>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<ResponseResult<long>> CreateChatAsync()
    {
        return await _chatService.CreateChatAsync();
    }

    /// <summary>
    /// 重命名聊天
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpPost("{id}/rename")]
    public async Task<ResponseResult<bool>> RenameChatAsync(long id, string name)
    {
        return await _chatService.RenameChatAsync(id, name);
    }

    /// <summary>
    /// 删除聊天
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id}/delete")]
    public async Task<ResponseResult<bool>> DeleteChatAsync(long id)
    {
        return await _chatService.DeleteChatAsync(id);
    }

    /// <summary>
    /// 恢复聊天
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost("{id}/restore")]
    public async Task<ResponseResult<bool>> RestoreChatAsync(long id)
    {
        return await _chatService.RestoreChatAsync(id);
    }

    /// <summary>
    /// 获取聊天记录
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
    /// 添加聊天机器人的设定
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="requestDtoDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{chatId}/system-message")]
    public async Task<ResponseResult<bool>> AddSystemMessageAsync(long chatId,
        [FromBody] AddSystemMessageRequestDto requestDtoDto,
        CancellationToken cancellationToken)
    {
        return await _chatService.AddSystemMessageAsync(chatId, requestDtoDto.SystemMessage, cancellationToken);
    }
}
