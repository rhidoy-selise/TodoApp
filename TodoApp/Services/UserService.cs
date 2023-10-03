using MongoDB.Driver;
using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Repository;

namespace TodoApp.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _user;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IMongoDatabase database,
        ILogger<UserService> logger
    )
    {
        _user = new Repository<User>(database);
        _logger = logger;
    }

    public async Task CreateAsync(UserCreateDto dto)
    {
        _logger.Log(LogLevel.Information, "User add call {}", dto);
        await _user.Insert(dto.GetUser());
    }

    public async Task<List<User>> GetAsync(int page, int pageSize)
    {
        return await _user.GetAll(page, pageSize);
    }

    public async Task<UserGetDto?> UpdateAsync(UserUpdateDto dto)
    {
        _logger.Log(LogLevel.Information, "User update call {}", dto);
        var entity = await _user.GetById(dto.Id);
        dto.UpdateUser(entity);

        await _user.Update(entity);

        return UserGetDto.GetUserDto(entity);
    }

    public async Task<UserGetDto?> GetById(Guid id)
    {
        var entity = await _user.GetById(id);
        return UserGetDto.GetUserDto(entity);
    }

    public async Task DeleteById(Guid id)
    {
        await _user.Delete(id);
    }
}
