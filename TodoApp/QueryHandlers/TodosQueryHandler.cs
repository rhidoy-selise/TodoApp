using Newtonsoft.Json;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.QueryHandlers;

public class TodosQueryHandler : IHandler<TodosQuery>
{
    private readonly ITodoService _service;
    private readonly ILogger<TodosQueryHandler> _logger;

    public TodosQueryHandler(ITodoService service, ILogger<TodosQueryHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public HandlerResponse Handle(TodosQuery signal)
    {
        return HandleAsync(signal).Result;
    }

    public async Task<HandlerResponse> HandleAsync(TodosQuery query)
    {
        _logger.LogInformation("Enter {TodoByIdHandlerName} with payload: {SerializeObject}",
            GetType().Name, JsonConvert.SerializeObject(query));
        try
        {
            var results = await _service.GetAsync(query);
            return new HandlerResponse(results);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
