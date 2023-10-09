using Newtonsoft.Json;
using TodoApp.Dto;
using TodoApp.Queries;
using TodoApp.Services;
using TodoApp.Utils;

namespace TodoApp.QueryHandlers;

public class GetTodoQueryHandler : IHandler<GetTodoQuery, PagedList<TodoGetDto>>
{
    private readonly ITodoService _service;
    private readonly ILogger<GetTodoQueryHandler> _logger;

    public GetTodoQueryHandler(ITodoService service, ILogger<GetTodoQueryHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public PagedList<TodoGetDto> Handle(GetTodoQuery query)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedList<TodoGetDto>> HandleAsync(GetTodoQuery query)
    {
        _logger.LogInformation("Enter {TodoByIdHandlerName} with payload: {SerializeObject}",
            nameof(GetTodoByIdQueryHandler), JsonConvert.SerializeObject(query));
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
