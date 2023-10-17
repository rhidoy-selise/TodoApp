using TodoApp.Commands;
using TodoApp.Dto;
using TodoApp.Queries;

namespace TodoApp.Services;

public interface ITodoService
{
    public Task<List<TodoResponse>> GetAsync(TodosQuery query);
    public Task<TodoResponse> GetAsync(Guid id);
    public Task<TodoResponse> AddAsync(AddTodoCommand dto);
    public Task<TodoResponse> UpdateAsync(UpdateTodoCommand dto);
    public Task Delete(Guid id);
}
