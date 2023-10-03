using TodoApp.Dto;
using TodoApp.Models;

namespace TodoApp.Services;

public interface IUserService
{
    public Task CreateAsync(UserCreateDto dto);
    public Task<List<User>> GetAsync(int page, int pageSize);
    public Task<UserGetDto?> UpdateAsync(UserUpdateDto dto);
    public Task DeleteById(Guid id);
}
