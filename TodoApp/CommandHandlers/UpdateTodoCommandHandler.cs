using Newtonsoft.Json;
using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.CommandHandlers;

public class UpdateTodoCommandHandler : IHandler<UpdateTodoCommand, TodoGetDto>
{
    private readonly ITodoService _service;
    private readonly ILogger<UpdateTodoCommandHandler> _logger;

    public UpdateTodoCommandHandler(ITodoService service, ILogger<UpdateTodoCommandHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public TodoGetDto Handle(UpdateTodoCommand query)
    {
        throw new NotImplementedException();
    }

    public async Task<TodoGetDto> HandleAsync(UpdateTodoCommand query)
    {
        _logger.LogInformation("Enter {} with payload: {}",
            GetType().Name, JsonConvert.SerializeObject(query));
        try
        {
            return await _service.UpdateAsync(query);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
