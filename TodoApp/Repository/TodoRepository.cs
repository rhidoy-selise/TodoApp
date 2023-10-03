using MongoDB.Bson;
using MongoDB.Driver;
using TodoApp.Dto;
using TodoApp.Models;

namespace TodoApp.Repository;

public class TodoRepository
{
    private readonly IMongoCollection<Todo> _collection;
    private readonly IMongoCollection<User> _collectionUser;

    public TodoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Todo>("todo");
        _collectionUser = database.GetCollection<User>("user");
    }

    public async Task CreateAsync(Todo todo)
    {
        await _collection.InsertOneAsync(todo);
    }

    public async Task<List<TodoGetDto>> GetAsync(
        int page, int pageSize
    )
    {
        var todos = await _collection
            .Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        var userIds = todos.Select(todo => todo.CreateUserGuid)
            .Concat(todos.Select(todo => todo.AssignedUserGuid))
            .Distinct()
            .ToList();

        // Fetch Users based on User IDs
        var users = await _collectionUser
            .Find(u => userIds.Contains(u.Id))
            .ToListAsync();

        return todos.Select(todo => new TodoGetDto
        {
            Id = todo.Id,
            Name = todo.Name,
            Description = todo.Description,
            Complete = todo.Complete,
            Status = todo.Status,
            CreateDate = todo.CreateDate,
            UpdateDate = todo.UpdateDate,
            CreatedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == todo.CreateUserGuid)),
            AssignedUser = UserGetDto.GetUserDto(users.FirstOrDefault(u => u.Id == todo.AssignedUserGuid))
        }).ToList();
    }
}
