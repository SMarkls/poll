namespace Poll.Core.Services.PollPage;

public interface IPollPageService
{
    Task<Entities.PollPage?> GetPollPage(string pollPageId, string pollId);
    Task<string> AddPollPage(Entities.PollPage pollPage, string pollId);
    Task UpdateHeader(string pollPageId, string pollId, string header);
    Task RemovePollPage(string pollPageId, string pollId);
    Task DeleteQuestion(string pollPageId, string pollId, string questionId);
}