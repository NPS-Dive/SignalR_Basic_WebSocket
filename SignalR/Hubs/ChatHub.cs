using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalR.Models;
using SignalR.Services;

namespace SignalR.Hubs;

public class ChatHub : Hub
    {
    private readonly IChatRoomService _chatRoomService;
    private readonly IHubContext<AgentHub> _agentHub;

    public ChatHub ( IChatRoomService chatRoomService, IHubContext<AgentHub> agentHub )
        {
        _chatRoomService = chatRoomService;
        _agentHub = agentHub;
        }

    public override async Task OnConnectedAsync ()
        {
        if (Context.User.Identity.IsAuthenticated)
            {
            await base.OnConnectedAsync();
            return;
            }

        var roomId = await _chatRoomService.CreateRoom(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

        await Clients.Caller.SendAsync(
            "ReceiveMessage",
            "IPBSES Support",
            DateTime.UtcNow.ToString("o"),
            "Hey there, welcome!");

        await base.OnConnectedAsync();
        }

    public async Task SendMessage ( string name, string message )
        {
        var roomId = await _chatRoomService.GetRoomForConnectionId(Context.ConnectionId);
        var chatMessage = new ChatMessage(name, DateTime.UtcNow.ToString("o"), message);

        await _chatRoomService.AddMessage(roomId, chatMessage);

        await Clients.Group(roomId.ToString()).SendAsync(
            "ReceiveMessage",
            chatMessage.SenderName,
            chatMessage.SentAt,
            chatMessage.Text);

        await _agentHub.Clients.All.SendAsync("ActiveRooms", await _chatRoomService.GetAllRooms());
        }

    public async Task SetName ( string visitorName )
        {
        var roomName = $"Chat with {visitorName} from IPBSES";
        var roomId = await _chatRoomService.GetRoomForConnectionId(Context.ConnectionId);

        await _chatRoomService.SetRoomName(roomId, roomName);
        await _agentHub.Clients.All.SendAsync("ActiveRooms", await _chatRoomService.GetAllRooms());
        }

    [Authorize]
    public async Task JoinRoom ( Guid roomId )
        {
        if (roomId == Guid.Empty)
            {
            throw new ArgumentException("Room Id cannot be empty");
            }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        }

    [Authorize]
    public async Task LeaveRoom ( Guid roomId )
        {
        if (roomId == Guid.Empty)
            {
            throw new ArgumentException("Room Id cannot be empty");
            }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }
    }