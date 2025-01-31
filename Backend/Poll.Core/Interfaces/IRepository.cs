using System.Linq.Expressions;

namespace Poll.Core.Interfaces;

public interface IRepository<T> where T : class
{
    Task<Entities.Poll?> GetById(string id, CancellationToken ct);
    Task<List<T>> GetAll(string userId, CancellationToken ct);
    Task<List<T>> GetByFilter(Expression<Func<T, bool>> filter, CancellationToken ct);
    Task<string> Add(T entity, CancellationToken ct);
    Task<List<string>> AddAll(List<T> entities, CancellationToken ct);
    Task<T> Update(T entity, CancellationToken ct);
    Task Delete(string id, CancellationToken ct);
}