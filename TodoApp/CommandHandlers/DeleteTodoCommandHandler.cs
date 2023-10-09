using Newtonsoft.Json;
using TodoApp.Commands;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.CommandHandlers;

public class DeleteTodoCommandHandler : IHandler<DeleteTodoCommand, Task>
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

    public Task Handle(DeleteTodoCommand query)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(query));
        try
        {
            _service.DeleteById(query.Id);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task<Task> HandleAsync(DeleteTodoCommand query)
    {
        throw new NotImplementedException();
    }
}
