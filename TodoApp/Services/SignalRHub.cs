using Microsoft.AspNetCore.SignalR;

namespace TodoApp.Services;

public class SignalRHub : Hub
{
    public async Task SendMessage(string topic, string message)
    {
        await Clients.All.SendAsync(topic, message);
    }
}
