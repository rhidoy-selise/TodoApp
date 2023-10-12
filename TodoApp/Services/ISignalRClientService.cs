using Microsoft.AspNetCore.SignalR.Client;

namespace TodoApp.Services;

public interface ISignalRClientService
{
    HubConnection? GetHubConnection();
}
