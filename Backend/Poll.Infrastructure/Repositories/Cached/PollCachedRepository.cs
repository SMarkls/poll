using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Poll.Core.Entities.Answers;
using Poll.Core.Exceptions;
using Poll.Core.Interfaces;
using Poll.Infrastructure.Extensions;

namespace Poll.Infrastructure.Repositories.Cached;

public class PollCachedRepository : IPollRepository
{
    private readonly IPollRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<PollCachedRepository> _logger;
    private const uint LifeTime = 60;

    public PollCachedRepository(IPollRepository repository, IDistributedCache cache, ILogger<PollCachedRepository> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Core.Entities.Poll?> GetById(string id, CancellationToken ct)
    {
        var value = await _cache.GetValue<Core.Entities.Poll>(id, ct, _logger);
        if (value is not null)
        {
            return value;
        }

        _logger.LogDebug("Значение по ключу {Key} в кэше не найдено", id);
        var result = await _repository.GetById(id, ct);
        if (result is null)
        {
            return null;
        }

        await _cache.SetValue(id, result, LifeTime, ct);
        return result;
    }

    public async Task<List<Core.Entities.Poll>> GetAll(string userId, CancellationToken ct)
    {
        var result = await _repository.GetAll(userId, ct);
        return result;
    }

    public async Task<List<Core.Entities.Poll>> GetByFilter(Expression<Func<Core.Entities.Poll, bool>> filter, CancellationToken ct)
    {
        var result = await _repository.GetByFilter(filter, ct);
        return result;
    }

    public async Task<string> Add(Core.Entities.Poll entity, CancellationToken ct)
    {
        var result = await _repository.Add(entity, ct);
        if (!string.IsNullOrEmpty(result))
        {
            await _cache.SetValue(result, entity, LifeTime, ct);
        }

        return result;
    }

    public async Task<List<string>> AddAll(List<Core.Entities.Poll> entities, CancellationToken ct)
    {
        var result = await _repository.AddAll(entities, ct);
        return result;
    }

    public async Task<Core.Entities.Poll> Update(Core.Entities.Poll entity, CancellationToken ct)
    {
        var result = await _repository.Update(entity, ct);
        await _cache.SetValue(entity.PollId, result,  LifeTime, ct);
        return result;
    }

    public async Task Delete(string id, CancellationToken ct)
    {
        await _repository.Delete(id, ct);
        await _cache.RemoveAsync(id, ct);
    }

    public async Task<string> GetOwnerId(string pollId, CancellationToken ct)
    {
        var owner = await _cache.GetValue<string>($"{pollId}_owner", ct);
        if (!string.IsNullOrEmpty(owner))
        {
            return owner;
        }

        var result = await _repository.GetOwnerId(pollId, ct);
        await _cache.SetValue($"{pollId}_owner", result, LifeTime, ct);
        return result;
    }

    public async Task Complete(string pollId, string userId, CompletePollDto dto, CancellationToken ct)
    {
        var value = await _cache.GetValue<Core.Entities.Poll>(pollId, ct, _logger);
        if (value is null)
        {
            await _repository.Complete(pollId, userId, dto, ct);
            await _cache.SetValue(pollId, await _repository.GetById(pollId, ct), LifeTime, ct);
        }
        else if (!value.IsAnswersEditable && value.PassedEmployees.Contains(userId))
        {
            throw new AppException("Ответы на этот опрос нельзя менять");
        }
        else
        {
            await _repository.Complete(pollId, userId, dto, ct);
            value.PassedEmployees.Add(userId);
            await _cache.SetValue(pollId, value, LifeTime, ct);
        }
    }

    public Task PersistProgress(string pollId, string userId, CompletePollDto dto, CancellationToken ct)
    {
        return _cache.SetValue($"{pollId}_{userId}", dto, 30, ct);
    }

    public Task<CompletePollDto?> GetProgress(string pollId, string userId, CancellationToken ct)
    {
        return _cache.GetValue<CompletePollDto>($"{pollId}_{userId}", ct);
    }

    public Task<Dictionary<string, string>?> GetEdit(string pollId, string userId, CancellationToken ct)
    {
        return _repository.GetEdit(pollId, userId, ct);
    }
}