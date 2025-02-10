using Poll.Core.Entities;
using Poll.Core.Interfaces;

namespace Poll.Core.Services.PollPage;

public class PollPageService : IPollPageService
{
    private readonly IPollPageRepository _repository;

    public PollPageService(IPollPageRepository repository)
    {
        _repository = repository;
    }

    public Task<Entities.PollPage?> GetPollPage(string pollPageId, string pollId, CancellationToken ct)
    {
        return _repository.GetPollPage(pollPageId, pollId, ct);
    }

    public Task<string> AddPollPage(Entities.PollPage pollPage, string pollId, CancellationToken ct)
    {
        return _repository.AddPollPage(pollPage, pollId, ct);
    }

    public Task UpdateHeader(string pollPageId, string pollId, string header, CancellationToken ct)
    {
        return _repository.UpdateHeader(pollPageId, pollId, header, ct);
    }

    public Task RemovePollPage(string pollPageId, string pollId, CancellationToken ct)
    {
        return _repository.RemovePollPage(pollPageId, pollId, ct);
    }

    public Task DeleteQuestion(string pollPageId, string pollId, string questionId, CancellationToken ct)
    {
        return _repository.DeleteQuestion(pollPageId, pollId, questionId, ct);
    }

    public Task EditQuestionText(string pollPageId, string pollId, string questionId, string text, CancellationToken ct)
    {
        return _repository.EditQuestionText(pollPageId, pollId, questionId, text, ct);
    }

    public Task<string> AddQuestion(string pollPageId, string pollId, Question question, CancellationToken ct)
    {
        return _repository.AddQuestion(pollPageId, pollId, question, ct);
    }
}