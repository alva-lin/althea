using System.Runtime.CompilerServices;
using Althea.Data.Domains.ChatDomain;
using Althea.Models;
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
        return await _chatService.GetChatsAsync(cancellationToken);
    }

    /// <summary>
    ///     获取聊天信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:long}")]
    public async Task<ResponseResult<ChatInfoDto>> GetChatInfoAsync(long id,
        CancellationToken cancellationToken = default)
    {
        return await _chatService.GetChatInfoAsync(id, cancellationToken);
    }

    /// <summary>
    ///     生成聊天名称
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id:long}/gen-title")]
    public async Task<ResponseResult<string>> GenerateTitleAsync(long id, CancellationToken cancellationToken)
    {
        return await _chatService.GenerateTitleAsync(id, cancellationToken);
    }

    /// <summary>
    ///     重命名聊天
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("{id:long}/rename")]
    public async Task<ResponseResult<bool>> RenameChatAsync(long id, RenameChatRequestDto dto)
    {
        return await _chatService.RenameChatAsync(id, dto.Title);
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
    ///     发送消息
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("send")]
    public async IAsyncEnumerable<ResponseResult<ChatResponse>> SendMessageAsync(SendMessageRequestDto dto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var result =
            _chatService.SendMessageAsync(dto.Message, dto.ChatId, dto.PrevMessageId, dto.Model, cancellationToken);
        await foreach (var received in result.WithCancellation(cancellationToken))
            yield return received;
    }
}
