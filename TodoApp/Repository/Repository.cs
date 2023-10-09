using System.Linq.Expressions;
using MongoDB.Driver;
using TodoApp.Utils;

namespace TodoApp.Repository;

public class Repository : IRepository
{
    private readonly IMongoDatabase _database;

    public Repository(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<List<T>> Get<T>(
        Expression<Func<T, bool>>? filters,
        Paging paging
    )
    {
        var filterDefinition = filters != null
            ? Builders<T>.Filter.Where(filters)
            : Builders<T>.Filter.Empty;

        return await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .Find(filterDefinition)
            .Skip(paging.GetOffset())
            .Limit(paging.PerPage)
            .ToListAsync();
    }

    public async Task<T> Get<T>(Expression<Func<T, bool>> filters)
    {
        return await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .Find(filters)
            .FirstOrDefaultAsync();
    }

    public async Task<T> Get<T>(Guid id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("The entity does not have an Id property.");
        }

        var filter = Builders<T>.Filter.Eq(idProperty.Name, id);
        var entity = await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .Find(filter)
            .FirstOrDefaultAsync();

        if (entity == null)
        {
            throw new KeyNotFoundException("Entity not found");
        }

        return entity;
    }

    public async Task Insert<T>(T entity)
    {
        await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .InsertOneAsync(entity);
    }

    public async Task Update<T>(Expression<Func<T, bool>> filters, T entity)
    {
        var result = await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .ReplaceOneAsync(filters, entity);

        if (!result.IsAcknowledged)
        {
            throw new Exception("Entity update failed");
        }
    }

    public async Task Update<T>(T entity)
    {
        var idProperty = entity?.GetType().GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("The entity does not have an Id property.");
        }

        var idValue = idProperty.GetValue(entity);
        var filter = Builders<T>.Filter.Eq("_id", idValue);
        var result = await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .ReplaceOneAsync(filter, entity);

        if (!result.IsAcknowledged)
        {
            throw new Exception("Entity update failed");
        }
    }

    public async Task Delete<T>(Expression<Func<T, bool>> filters)
    {
        var result = await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .DeleteOneAsync(filters);
        if (result.DeletedCount == 0)
        {
            throw new Exception("Entity delete failed");
        }
    }

    public async Task Delete<T>(Guid id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("The entity does not have an Id property.");
        }

        var filter = Builders<T>.Filter.Eq(idProperty.Name, id);
        var result = await _database
            .GetCollection<T>(typeof(T).Name.ToLower())
            .DeleteOneAsync(filter);
        if (result.DeletedCount == 0)
        {
            throw new Exception("Entity delete failed");
        }
    }
}
