using Microsoft.AspNetCore.SignalR.Client;

namespace TodoApp.Services;

public class SignalRClientService : ISignalRClientService, IDisposable
{
    private readonly ILogger<SignalRClientService> _logger;
    private readonly SignalRClientHostedService _hostedService;

    public SignalRClientService(
        ILogger<SignalRClientService> logger,
        SignalRClientHostedService hostedService)
    {
        _logger = logger;
        _hostedService = hostedService;
        StartSocketClient();
    }
        

    private async void StartSocketClient()
    {
        await _hostedService.StartAsync(CancellationToken.None);
    }

    public HubConnection GetHubConnection()
    {
        return _hostedService.GetHubConnection();
    }

    public async void Dispose()
    {
       await _hostedService.StopAsync(CancellationToken.None);
    }
}
