using Newtonsoft.Json;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.QueryHandlers;

public class GetTodoByIdQueryHandler : IHandler<GetTodoByIdQuery, TodoGetDto>
{
    private readonly ITodoService _service;
    private readonly ILogger<GetTodoByIdQueryHandler> _logger;

    public GetTodoByIdQueryHandler(
        ITodoService service,
        ILogger<GetTodoByIdQueryHandler> logger
    )
    {
        _service = service;
        _logger = logger;
    }

    public TodoGetDto Handle(GetTodoByIdQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<TodoGetDto> HandleAsync(GetTodoByIdQuery query)
    {
        _logger.LogInformation("Enter {TodoByIdHandlerName} with payload: {SerializeObject}",
            nameof(GetTodoByIdQueryHandler), JsonConvert.SerializeObject(query));
        try
        {
            return await _service.GetById(query.Id);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
