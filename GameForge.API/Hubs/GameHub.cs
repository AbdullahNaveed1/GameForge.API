using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GameForge.API.Hubs;

[Authorize]
public class GameHub : Hub
{
    private static readonly HashSet<string> ConnectedUsers = new();

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.Identity?.Name ?? "Adventurer";
        ConnectedUsers.Add(username);

        // Notify all players that a new hero has joined the lobby
        await Clients.Others.SendAsync("PlayerJoined", username);
        await Clients.Caller.SendAsync("OnlinePlayersList", ConnectedUsers.ToList());

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.Identity?.Name ?? "Adventurer";
        ConnectedUsers.Remove(username);

        // Notify remaining players
        await Clients.Others.SendAsync("PlayerLeft", username);

        await base.OnDisconnectedAsync(exception);
    }

    // Broadcast global chat messages
    public async Task SendGlobalMessage(string message)
    {
        var username = Context.User?.Identity?.Name ?? "Adventurer";
        var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");

        await Clients.All.SendAsync("ReceiveGlobalMessage", username, message, timestamp);
    }

    // Direct / Party room messaging
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        var username = Context.User?.Identity?.Name ?? "Adventurer";
        await Clients.Group(roomName).SendAsync("ReceiveRoomMessage", "System", $"{username} joined {roomName}.");
    }

    public async Task SendRoomMessage(string roomName, string message)
    {
        var username = Context.User?.Identity?.Name ?? "Adventurer";
        var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");

        await Clients.Group(roomName).SendAsync("ReceiveRoomMessage", username, message, timestamp);
    }
}