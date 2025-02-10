namespace Poll.Core.Services.ViewRule;

public interface IRuleService
{
    Task<string> CreateOrUpdateRule(Entities.ViewRules.ViewRule rule, string pollId, string pollPageId, string questionId, 
        CancellationToken ct);
    Task DeleteRule(string pollId, string pollPageId, string questionId, CancellationToken ct);
}