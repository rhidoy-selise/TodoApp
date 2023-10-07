using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace TodoApp.Services;

public class SignalRService : ISignalRService
{
    private readonly ILogger<SignalRService> _logger;
    private readonly IHubContext<SignalRHub> _hubContext;

    public SignalRService(
        ILogger<SignalRService> logger,
        IHubContext<SignalRHub> hubContext
    )
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task SendMessage<T>(string topic, T message)
    {
        try
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            await _hubContext.Clients.All.SendAsync(topic, jsonMessage);
            _logger.Log(LogLevel.Information, "Socket message sent to topic {}, Message: {}", topic, jsonMessage);
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, "Socket message sent error. Message: {}", e.Message);
        }
    }
}
