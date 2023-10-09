using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Queries;

namespace TodoApp.Services;

public interface ITodoService
{
    public Task<TodoGetDto> CreateAsync(AddTodoCommand dto);
    public Task<List<TodoGetDto>> GetAsync(GetTodoQuery query);
    public Task<TodoGetDto> GetById(Guid id);
    public Task DeleteById(Guid id);
    public Task<TodoGetDto> UpdateAsync(UpdateTodoCommand dto);
}
