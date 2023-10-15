using Newtonsoft.Json;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.CommandHandlers;

public class AddTodoCommandHandler : IHandler<AddTodoCommand, TodoGetDto>
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

    public TodoGetDto Handle(AddTodoCommand query)
    {
        throw new NotImplementedException();
    }

    public async Task<TodoGetDto> HandleAsync(AddTodoCommand query)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(query));
        try
        {
            return await _service.AddAsync(query);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
