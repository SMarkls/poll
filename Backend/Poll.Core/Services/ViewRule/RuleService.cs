using Poll.Core.Interfaces;

namespace Poll.Core.Services.ViewRule;

public class RuleService : IRuleService
{
    private readonly IRuleRepository _repository;

    public RuleService(IRuleRepository repository)
    {
        _repository = repository;
    }

    public Task<string> CreateOrUpdateRule(Entities.ViewRules.ViewRule rule, string pollId, string pollPageId, string questionId, CancellationToken ct)
    {
        return _repository.CreateOrUpdateRule(rule, pollId, pollPageId, questionId, ct);
    }

    public Task DeleteRule(string pollId, string pollPageId, string questionId, CancellationToken ct)
    {
        return _repository.DeleteRule(pollId, pollPageId, questionId, ct);
    }
}
