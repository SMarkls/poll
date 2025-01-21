using Poll.Core.Entities;

namespace Poll.Core.Interfaces;

public interface IPollPageRepository
{
    Task<string> AddPollPage(PollPage pollPage, string pollId);
    Task RemovePollPage(string pollPageId, string pollId);
    Task UpdateHeader(string pollPageId, string pollId, string headerValue);
    Task<PollPage?> GetPollPage(string pollPageId, string pollId);
    Task DeleteQuestion(string pollPageId, string pollId, string questionId);
}