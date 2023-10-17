using Newtonsoft.Json;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.CommandHandlers;

public class AddTodoCommandHandler : IHandler<AddTodoCommand>
{
    private readonly ITodoService _service;
    private readonly ILogger<AddTodoCommandHandler> _logger;

    public AddTodoCommandHandler(
        ITodoService service,
        ILogger<AddTodoCommandHandler> logger
    )
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(AddTodoCommand signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(AddTodoCommand signal)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(signal));
        try
        {
            var result = await _service.AddAsync(signal);
            return new HandlerResponse(new List<object> { result });
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
