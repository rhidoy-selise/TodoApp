using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Repository;
using TodoApp.Utils;

namespace TodoApp.Services;

public class UserService : IUserService
{
    private readonly IRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        ILogger<UserService> logger,
        IRepository repository
    )
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task CreateAsync(UserCreateDto dto)
    {
        _logger.Log(LogLevel.Information, "User add call {}", dto);
        await _repository.Insert(dto.GetUser());
    }

    public async Task<List<User>> GetAsync(Paging paging)
    {
        return await _repository.Get<User>(null, paging);
    }

    public async Task<UserGetDto?> UpdateAsync(UserUpdateDto dto)
    {
        _logger.Log(LogLevel.Information, "User update call {}", dto);
        var entity = await _repository.Get<User>(dto.Id);
        dto.UpdateUser(entity);

        await _repository.Update(entity);

        return UserGetDto.GetUserDto(entity);
    }

    public async Task<UserGetDto> GetById(Guid id)
    {
        var entity = await _repository.Get<User>(id);
        return UserGetDto.GetUserDto(entity);
    }

    public async Task DeleteById(Guid id)
    {
        await _repository.Delete<User>(id);
    }
}
