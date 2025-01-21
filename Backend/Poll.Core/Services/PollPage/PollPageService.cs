using Poll.Core.Interfaces;

namespace Poll.Core.Services.PollPage;

public class PollPageService : IPollPageService
{
    private readonly IPollPageRepository _repository;

    public PollPageService(IPollPageRepository repository)
    {
        _repository = repository;
    }

    public Task<Entities.PollPage?> GetPollPage(string pollPageId, string pollId)
    {
        return _repository.GetPollPage(pollPageId, pollId);
    }

    public Task<string> AddPollPage(Entities.PollPage pollPage, string pollId)
    {
        return _repository.AddPollPage(pollPage, pollId);
    }

    public Task UpdateHeader(string pollPageId, string pollId, string header)
    {
        return _repository.UpdateHeader(pollPageId, pollId, header);
    }

    public Task RemovePollPage(string pollPageId, string pollId)
    {
        return _repository.RemovePollPage(pollPageId, pollId);
    }

    public Task DeleteQuestion(string pollPageId, string pollId, string questionId)
    {
        return _repository.DeleteQuestion(pollPageId, pollId, questionId);
    }
}