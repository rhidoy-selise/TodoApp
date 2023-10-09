using System.Linq.Expressions;
using TodoApp.Utils;

namespace TodoApp.Repository;

public interface IRepository
{
    public Task<List<T>> Get<T>(Expression<Func<T, bool>>? dataFilters, Paging paging);
    public Task<T> Get<T>(Expression<Func<T, bool>> filters);
    public Task<T> Get<T>(Guid id);
    public Task Insert<T>(T entity);
    public Task Update<T>(Expression<Func<T, bool>> filters, T entity);
    public Task Update<T>(T entity);
    public Task Delete<T>(Expression<Func<T, bool>> filters);
    public Task Delete<T>(Guid id);
}
