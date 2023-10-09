using TodoApp.Dto;
using TodoApp.Models;
using TodoApp.Utils;

namespace TodoApp.Services;

public interface IUserService
{
    public Task CreateAsync(UserCreateDto dto);
    public Task<List<User>> GetAsync(Paging paging);
    public Task<UserGetDto?> UpdateAsync(UserUpdateDto dto);
    public Task<UserGetDto> GetById(Guid id);
    public Task DeleteById(Guid id);
}
