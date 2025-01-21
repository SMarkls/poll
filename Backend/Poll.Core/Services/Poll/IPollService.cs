namespace Poll.Core.Services.Poll;

public interface IPollService
{
    Task<string> AddPoll(Entities.Poll poll);
    Task Delete(string pollId);
    Task<Entities.Poll?> Get(string pollId);
    Task Update(Entities.Poll poll);
    Task<List<Entities.Poll>> GetAll(string userId);
}