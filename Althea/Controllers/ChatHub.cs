using System.Runtime.CompilerServices;
using Althea.Data.Domains.ChatDomain;
using Althea.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Althea.Controllers;

// [Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;
    private readonly IAuthInfoProvider _authInfoProvider;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger, IAuthInfoProvider authInfoProvider)
    {
        _chatService = chatService;
        _logger = logger;
        _authInfoProvider = authInfoProvider;
    }

    private void SetAuthInfo()
    {
        _authInfoProvider.SetCurrentUser(Context.UserIdentifier ?? _authInfoProvider.CurrentUser);
    }

    /// <summary>
    ///     发送消息
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<ResponseResult<ChatResponse>> SendMessage(SendMessageRequestDto dto,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        SetAuthInfo();
        var result =
            _chatService.SendMessageAsync(dto.Message, dto.ChatId, dto.PrevMessageId, dto.Model, cancellationToken);
        await foreach (var received in result.WithCancellation(cancellationToken))
            yield return received;
    }
}
