using Poll.Core.Entities;

namespace Poll.Core.Services.PollPage;

public interface IPollPageService
{
    Task<Entities.PollPage?> GetPollPage(string pollPageId, string pollId, CancellationToken ct);
    Task<string> AddPollPage(Entities.PollPage pollPage, string pollId, CancellationToken ct);
    Task UpdateHeader(string pollPageId, string pollId, string header, CancellationToken ct);
    Task RemovePollPage(string pollPageId, string pollId, CancellationToken ct);
    Task DeleteQuestion(string pollPageId, string pollId, string questionId, CancellationToken ct);
    Task EditQuestionText(string pollPageId, string pollId, string questionId, string text, CancellationToken ct);
    Task<string> AddQuestion(string pollPageId, string pollId, Question question, CancellationToken ct);
}