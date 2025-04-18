using SignalR.Models;

namespace SignalR.Services;

public class InMemoryChatRoomService : IChatRoomService
{
    private readonly Dictionary<Guid, ChatRoom> _roomInfo = new();
    private readonly Dictionary<Guid, List<ChatMessage>> _messageHistory = new();

    public async Task<Guid> CreateRoom ( string connectionId )
    {
        var id = Guid.NewGuid();
        _roomInfo[id] = new ChatRoom
        {
            OwnerConnectionId = connectionId
        };
        return await Task.FromResult(id);
    }

    public async Task<Guid> GetRoomForConnectionId ( string connectionId )
    {
        var foundRoom = _roomInfo.FirstOrDefault(x => x.Value.OwnerConnectionId == connectionId);
        if (foundRoom.Key == Guid.Empty)
        {
            throw new ArgumentException("Invalid Connection Id");
        }
        return await Task.FromResult(foundRoom.Key);
    }

    public Task SetRoomName ( Guid roomId, string name )
    {
        if (!_roomInfo.ContainsKey(roomId))
        {
            throw new ArgumentException("Invalid Room Id");
        }
        _roomInfo[roomId].Name = name;
        return Task.CompletedTask;
    }

    public Task AddMessage ( Guid roomId, ChatMessage message )
    {
        if (!_messageHistory.ContainsKey(roomId))
        {
            _messageHistory[roomId] = new List<ChatMessage>();
        }
        _messageHistory[roomId].Add(message);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ChatMessage>> GetMessageHistory ( Guid roomId )
    {
        _messageHistory.TryGetValue(roomId, out var messages);
        messages ??= new List<ChatMessage>();
        var sortedMessages = messages.OrderBy(msg => msg.SentAt).AsEnumerable(); // Chronological order
        return Task.FromResult(sortedMessages);
    }

    public Task<IReadOnlyDictionary<Guid, ChatRoom>> GetAllRooms ()
    {
        return Task.FromResult(_roomInfo as IReadOnlyDictionary<Guid, ChatRoom>);
    }
}