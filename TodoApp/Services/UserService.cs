using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Repository;

namespace TodoApp.Services;

public class UserService
{
    private readonly UserRepository _repository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserRepository userRepository,
        ILogger<UserService> logger
        )
    {
        _repository = userRepository;
        _logger = logger;
    }

    public async Task CreateAsync(UserCreateDto dto)
    {
        _logger.Log(LogLevel.Information, "User add call {}", dto);
        await _repository.CreateAsync(dto.GetUser());
    }

    public async Task<List<User>> GetAsync(int page, int pageSize)
    {
        return await _repository.GetAsync(page, pageSize);
    }

    public async Task<User> UpdateAsync(UserUpdateDto dto)
    {
        _logger.Log(LogLevel.Information, "User update call {}", dto);
        return await _repository.UpdateAsync(dto);
    }
}
