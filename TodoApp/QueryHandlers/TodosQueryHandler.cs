using Newtonsoft.Json;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.QueryHandlers;

public class TodosQueryHandler : IHandler<TodosQuery, PagedList<TodoGetDto>>
{
    private readonly ITodoService _service;
    private readonly ILogger<TodosQueryHandler> _logger;

    public TodosQueryHandler(ITodoService service, ILogger<TodosQueryHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public PagedList<TodoGetDto> Handle(TodosQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedList<TodoGetDto>> HandleAsync(TodosQuery query)
    {
        _logger.LogInformation("Enter {TodoByIdHandlerName} with payload: {SerializeObject}",
            GetType().Name, JsonConvert.SerializeObject(query));
        try
        {
            var list = await _service.GetAsync(query);
            return PagedList<TodoGetDto>.GetPagedList(list, query);
        }
        catch (Exception e)
        {
            _logger.LogError("Exception found {}", e.Message);
            throw;
        }
    }
}
