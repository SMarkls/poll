using System.Linq.Expressions;

namespace Poll.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<Entities.Poll?> GetById(string id);
    Task<List<T>> GetAll(string userId);
    Task<List<T>> GetByFilter(Expression<Func<T, bool>> filter);
    Task<string> Add(T entity);
    Task<List<string>> AddAll(List<T> entities);
    Task<T> Update(T entity);
    Task Delete(string id);
}