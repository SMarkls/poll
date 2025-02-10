using Poll.Core.Entities;

namespace Poll.Core.Interfaces;

public interface IPollPageRepository
{
    Task<string> AddPollPage(PollPage pollPage, string pollId, CancellationToken ct);
    Task RemovePollPage(string pollPageId, string pollId, CancellationToken ct);
    Task UpdateHeader(string pollPageId, string pollId, string headerValue, CancellationToken ct);
    Task<PollPage?> GetPollPage(string pollPageId, string pollId, CancellationToken ct);
    Task DeleteQuestion(string pollPageId, string pollId, string questionId, CancellationToken ct);
    Task EditQuestionText(string pollPageId, string pollId, string questionId, string newHeader, CancellationToken ct);
    Task<string> AddQuestion(string pollPageId, string pollId, Question question, CancellationToken ct);
}