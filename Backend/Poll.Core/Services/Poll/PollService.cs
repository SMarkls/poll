using Poll.Core.Interfaces;

namespace Poll.Core.Services.Poll;

public class PollService : IPollService
{
    private readonly IRepository<Entities.Poll> _repository;

    public PollService(IRepository<Entities.Poll> repository)
    {
        _repository = repository;
    }

    public Task<string> AddPoll(Entities.Poll poll)
    {
        return _repository.Add(poll);
    }

    public Task Delete(string pollId)
    {
        return _repository.Delete(pollId);
    }

    public Task<Entities.Poll?> Get(string pollId)
    {
        return _repository.GetById(pollId);
    }

    public Task Update(Entities.Poll poll)
    {
       return _repository.Update(poll);
    }

    public Task<List<Entities.Poll>> GetAll(string userId)
    {
        return _repository.GetAll(userId);
    }
}