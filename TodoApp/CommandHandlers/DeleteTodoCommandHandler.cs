using Newtonsoft.Json;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.CommandHandlers;

public class DeleteTodoCommandHandler : IHandler<DeleteTodoCommand>
{
    private readonly ITodoService _service;
    private readonly ILogger<DeleteTodoCommandHandler> _logger;

    public DeleteTodoCommandHandler(
        ITodoService service,
        ILogger<DeleteTodoCommandHandler> logger
    )
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(DeleteTodoCommand signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(DeleteTodoCommand signal)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(signal));
        try
        {
            await _service.Delete(signal.Id);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }

        return new HandlerResponse(null);
    }
}
