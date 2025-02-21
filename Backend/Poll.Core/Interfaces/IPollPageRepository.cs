using Poll.Core.Entities;

namespace Poll.Core.Interfaces;

public interface IPollPageRepository
{
    Task<string> AddPollPage(string pollId, PollPage pollPage, CancellationToken ct);
    Task RemovePollPage(string pollId, string pollPageId, CancellationToken ct);
    Task UpdateHeader(string pollId, string pollPageId, string headerValue, CancellationToken ct);
    Task<PollPage?> GetPollPage(string pollId, string pollPageId, CancellationToken ct);
    Task DeleteQuestion(string pollId, string pollPageId, string questionId, CancellationToken ct);
    Task UpdateQuestion(string pollId, string pollPageId, string questionId, Question entity, CancellationToken ct);
    Task<string> AddQuestion(string pollId, string pollPageId, Question question, CancellationToken ct);
}