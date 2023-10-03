using MongoDB.Driver;

namespace TodoApp.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public Repository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(typeof(T).Name.ToLower());
    }

    public IMongoCollection<T> GetCollection()
    {
        return _collection;
    }

    public async Task<List<T>> GetAll(int page, int pageSize)
    {
        var findOptions = new FindOptions<T>
        {
            Skip = (page - 1) * pageSize,
            Limit = pageSize
        };

        var cursor = await _collection.FindAsync(_ => true, findOptions);
        return await cursor.ToListAsync();
    }

    public async Task<T> GetById(Guid id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("The entity does not have an Id property.");
        }

        var filter = Builders<T>.Filter.Eq(idProperty.Name, id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();

        if (entity == null)
        {
            throw new KeyNotFoundException("Entity not found");
        }

        return entity;
    }

    public async Task Insert(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Update(T entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("The entity does not have an Id property.");
        }

        var idValue = idProperty.GetValue(entity);
        var filter = Builders<T>.Filter.Eq("_id", idValue);
        var result = await _collection.ReplaceOneAsync(filter, entity);

        if (!result.IsAcknowledged)
        {
            throw new Exception("Entity update failed");
        }
    }

    public async Task Delete(Guid id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null)
        {
            throw new ArgumentException("The entity does not have an Id property.");
        }

        var filter = Builders<T>.Filter.Eq(idProperty.Name, id);
        var result = await _collection.DeleteOneAsync(filter);
        if (result.DeletedCount == 0)
        {
            throw new Exception("Entity delete failed");
        }
    }
}
