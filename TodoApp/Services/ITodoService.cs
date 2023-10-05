using TodoApp.Dto;

namespace TodoApp.Services;

public interface ITodoService
{
    public Task CreateAsync(TodoCreateDto todo);
    public Task<List<TodoGetDto>> GetAsync(int page, int pageSize);
    public Task<TodoGetDto> GetById(Guid id);
    public Task DeleteById(Guid id);
    public Task<TodoGetDto> UpdateAsync(TodoUpdateDto dto);
}
