using MongoDB.Driver;

namespace TodoApp.Repository;

public interface IRepository<T>
{
    public IMongoCollection<T> GetCollection();
    public Task<List<T>> GetAll(int page, int pageSize);
    public Task<T> GetById(Guid id);
    public Task Insert(T entity);
    public Task Update(T entity);
    public Task Delete(Guid id);
}
