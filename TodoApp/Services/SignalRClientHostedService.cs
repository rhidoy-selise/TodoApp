using Microsoft.AspNetCore.SignalR.Client;

namespace TodoApp.Services;

public class SignalRClientHostedService : IHostedService
{
    private readonly ILogger<SignalRClientHostedService> _logger;
    private readonly HubConnection _hubConnection;

    public SignalRClientHostedService(
        IConfiguration configuration,
        ILogger<SignalRClientHostedService> logger
    )
    {
        _logger = logger;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(configuration["SignalRHubUrl"] ?? "http://localhost:5067/socket")
            .Build();
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Client Socket connection starting");

            await _hubConnection.StartAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, "Socket connection error {}", e.Message);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.Log(LogLevel.Information, "Stopping Websocket");
            await _hubConnection.StopAsync(cancellationToken);
            _logger.Log(LogLevel.Information, "Websocket Stopped");
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, "Stopping Websocket error {}", e.Message);
        }
    }

    public HubConnection GetHubConnection()
    {
        return _hubConnection;
    }
}
