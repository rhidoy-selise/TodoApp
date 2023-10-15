using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Queries;

namespace TodoApp.Services;

public interface ITodoService
{
    public Task<List<TodoGetDto>> GetAsync(TodosQuery query);
    public Task<TodoGetDto> GetAsync(Guid id);
    public Task<TodoGetDto> AddAsync(AddTodoCommand dto);
    public Task<TodoGetDto> UpdateAsync(UpdateTodoCommand dto);
    public Task Delete(Guid id);
}
