using MongoDB.Bson;
using MongoDB.Driver;
using TodoApp.Dto;
using TodoApp.Models;

namespace TodoApp.Repository;

public class UserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("user");
    }

    public async Task CreateAsync(User user)
    {
        await _collection.InsertOneAsync(user);
    }

    public async Task<List<User>> GetAsync(int page, int pageSize)
    {
        return await _collection
            .Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<User> GetById(Guid id)
    {
        var user = await _collection.Find(u => u.Id == id).FirstOrDefaultAsync();

        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        return user;
    }

    public async Task<User> UpdateAsync(UserUpdateDto dto)
    {
        var user = await GetById(dto.Id);
        user.Name = dto.Name;
        user.UpdateDate = DateTime.Now;

        var result = await _collection.ReplaceOneAsync(u => u.Id == dto.Id, user);
        if (!result.IsAcknowledged)
        {
            throw new Exception("User update failed");
        }

        return user;
    }
}
