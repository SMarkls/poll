using System.Linq.Expressions;
using Poll.Core.Entities.Answers;

namespace Poll.Core.Interfaces;

public interface IPollRepository
{
    Task<Entities.Poll?> GetById(string id, CancellationToken ct);
    Task<List<Core.Entities.Poll>> GetAll(string userId, CancellationToken ct);
    Task<List<Core.Entities.Poll>> GetByFilter(Expression<Func<Core.Entities.Poll, bool>> filter, CancellationToken ct);
    Task<string> Add(Core.Entities.Poll entity, CancellationToken ct);
    Task<List<string>> AddAll(List<Core.Entities.Poll> entities, CancellationToken ct);
    Task<Core.Entities.Poll> Update(Core.Entities.Poll entity, CancellationToken ct);
    Task Delete(string id, CancellationToken ct);
    Task<string> GetOwnerId(string pollId, CancellationToken ct);
    Task Complete(string pollId, string userId, CompletePollDto dto, CancellationToken ct);
    Task PersistProgress(string pollId, string userId, CompletePollDto dto, CancellationToken ct);
    Task<CompletePollDto?> GetProgress(string pollId, string userId, CancellationToken ct);
}