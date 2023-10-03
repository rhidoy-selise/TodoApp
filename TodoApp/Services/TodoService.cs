using MongoDB.Driver;
using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Repository;

namespace TodoApp.Services;

public class TodoService : ITodoService
{
    private readonly ILogger<TodoService> _logger;
    private readonly IRepository<Todo> _todo;
    private readonly IRepository<User> _user;

    public TodoService(
        ILogger<TodoService> logger,
        IMongoDatabase database
    )
    {
        _logger = logger;
        _todo = new Repository<Todo>(database);
        _user = new Repository<User>(database);
    }

    public async Task CreateAsync(TodoCreateDto dto)
    {
        await _todo.Insert(dto.GetTodo());
    }

    public async Task<List<TodoGetDto>> GetAsync(int page, int pageSize)
    {
        _logger.Log(LogLevel.Information, "Todo get call");
        var todos = await _todo.GetAll(page, pageSize);

        var userIds = todos.Select(todo => todo.CreateUserGuid)
            .Concat(todos.Select(todo => todo.AssignedUserGuid))
            .Distinct()
            .ToList();

        var users = await GetUsers(userIds);

        return todos
            .Select(todo => GetDto(todo, users).Result)
            .ToList();
    }

    public async Task<TodoGetDto> GetById(Guid id)
    {
        var entity = await _todo.GetById(id);
        return GetDto(entity, null).Result;
    }

    public async Task DeleteById(Guid id)
    {
        await _todo.Delete(id);
    }

    public async Task<TodoGetDto> UpdateAsync(TodoUpdateDto dto)
    {
        _logger.Log(LogLevel.Information, "Todo update call {}", dto);
        var entity = await _todo.GetById(dto.Id);
        dto.UpdateTodo(entity);

        await _todo.Update(entity);

        return GetDto(entity, null).Result;
    }

    private async Task<TodoGetDto> GetDto(Todo entity, List<User>? users)
    {
        if (users == null)
        {
            var userIds = new List<Guid>
            {
                entity.CreateUserGuid,
                entity.AssignedUserGuid
            };
            users = await GetUsers(userIds.Distinct().ToList());
        }

        var dto = TodoGetDto.GetDto(entity);
        dto.CreatedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == entity.CreateUserGuid));
        dto.AssignedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == entity.AssignedUserGuid));

        return dto;
    }

    private Task<List<User>> GetUsers(ICollection<Guid> userIds)
    {
        return _user
            .GetCollection()
            .Find(u => userIds.Contains(u.Id))
            .ToListAsync();
    }
}
