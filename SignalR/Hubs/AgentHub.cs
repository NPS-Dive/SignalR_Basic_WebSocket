using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;
using SignalR.Services;

namespace SignalR.Hubs;

[Authorize]
public class AgentHub : Hub
{
    private readonly IChatRoomService _chatRoomService;
    private readonly IHubContext<ChatHub> _chatHub;

    public AgentHub ( IChatRoomService chatRoomService, IHubContext<ChatHub> chatHub )
    {
        _chatRoomService = chatRoomService;
        _chatHub = chatHub;
    }

    public override async Task OnConnectedAsync ()
    {
        await Clients.Caller.SendAsync("ActiveRooms", await _chatRoomService.GetAllRooms());
        await base.OnConnectedAsync();
    }

    public async Task SendAgentMessages ( Guid roomId, string text )
    {
        var message = new ChatMessage(
            name: Context.User.Identity.Name,
            sentAt: DateTime.UtcNow.ToString("o"),
            text: text);

        await _chatRoomService.AddMessage(roomId, message);

        await _chatHub.Clients
            .Group(roomId.ToString())
            .SendAsync("ReceiveMessage", message.SenderName, message.SentAt, message.Text);
    }

    public async Task LoadHistory ( Guid roomId )
    {
        var history = await _chatRoomService.GetMessageHistory(roomId);
        // Log for debugging
        Console.WriteLine($"Sending history for room {roomId}: {history.Count()} messages");
        await Clients.Caller.SendAsync("ReceiveMessages", history);
    }
}