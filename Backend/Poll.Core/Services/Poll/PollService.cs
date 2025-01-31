using Poll.Core.Interfaces;

namespace Poll.Core.Services.Poll;

public class PollService : IPollService
{
    private readonly IRepository<Entities.Poll> _repository;

    public PollService(IRepository<Entities.Poll> repository)
    {
        _repository = repository;
    }

    public Task<string> AddPoll(Entities.Poll poll, CancellationToken ct)
    {
        return _repository.Add(poll, ct);
    }

    public Task Delete(string pollId, CancellationToken ct)
    {
        return _repository.Delete(pollId, ct);
    }

    public Task<Entities.Poll?> Get(string pollId, CancellationToken ct)
    {
        return _repository.GetById(pollId, ct);
    }

    public Task Update(Entities.Poll poll, CancellationToken ct)
    {
       return _repository.Update(poll, ct);
    }

    public Task<List<Entities.Poll>> GetAll(string userId, CancellationToken ct)
    {
        return _repository.GetAll(userId, ct);
    }
}