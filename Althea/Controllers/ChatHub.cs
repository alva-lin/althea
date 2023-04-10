using Althea.Data.Domains.ChatDomain;
using Microsoft.AspNetCore.SignalR;

// ReSharper disable UnusedMember.Global

namespace Althea.Controllers;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    public async IAsyncEnumerable<ChatResponse> SendMessage(long id, string sent)
    {
        await foreach (var received in _chatService.SendMessageAsync(id, sent, CancellationToken.None))
        {
            yield return received;
        }
    }
}
