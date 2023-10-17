using Newtonsoft.Json;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.CommandHandlers;

public class UpdateTodoCommandHandler : IHandler<UpdateTodoCommand>
{
    private readonly ITodoService _service;
    private readonly ILogger<UpdateTodoCommandHandler> _logger;

    public UpdateTodoCommandHandler(ITodoService service, ILogger<UpdateTodoCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(UpdateTodoCommand signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(UpdateTodoCommand signal)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(signal));
        try
        {
            var result = await _service.UpdateAsync(signal);
            return new HandlerResponse(new List<object> { result });
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
