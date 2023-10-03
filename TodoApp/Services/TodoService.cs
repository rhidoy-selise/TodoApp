using TodoApp.Dto;
using TodoApp.Repository;

namespace TodoApp.Services;

public class TodoService
{
    private readonly TodoRepository _todoRepository;
    private readonly ILogger<TodoService> _logger;

    public TodoService(
        TodoRepository todoRepository,
        ILogger<TodoService> logger
    )
    {
        _todoRepository = todoRepository;
        _logger = logger;
    }

    public async Task CreateAsync(TodoCreateDto dto)
    {
        await _todoRepository.CreateAsync(dto.GetTodo());
    }

    public async Task<List<TodoGetDto>> GetAsync(int page, int pageSize)
    {
        return await _todoRepository.GetAsync(page, pageSize);
    }
}
