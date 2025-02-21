using Poll.Core.Entities.ViewRules;

namespace Poll.Core.Interfaces;

public interface IRuleRepository
{
    Task<string> CreateOrUpdateRule(ViewRule rule, string pollId, string pollPageId, string questionId, CancellationToken ct);
    Task DeleteRule(string pollId, string pollPageId, string questionId, CancellationToken ct);
}