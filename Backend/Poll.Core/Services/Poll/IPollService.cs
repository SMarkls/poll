namespace Poll.Core.Services.Poll;

public interface IPollService
{
    Task<string> AddPoll(Entities.Poll poll, CancellationToken ct);
    Task Delete(string pollId, CancellationToken ct);
    Task<Entities.Poll?> Get(string pollId, CancellationToken ct);
    Task Update(Entities.Poll poll, CancellationToken ct);
    Task<List<Entities.Poll>> GetAll(string userId, CancellationToken ct);
}