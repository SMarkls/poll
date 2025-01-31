using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Poll.Core.Interfaces;
using Poll.Infrastructure.MongoConnection;

namespace Poll.Infrastructure.Repositories;

public class PollRepository
    : IRepository<Core.Entities.Poll>
{
    private readonly ILogger<PollRepository> _logger;
    private readonly IMongoCollection<Core.Entities.Poll> _collection;

    public PollRepository(IMongoCollectionFactory<Core.Entities.Poll> collectionFactory, ILogger<PollRepository> logger)
    {
        _collection = collectionFactory.GetCollection();
        _logger = logger;
    }
    
    public async Task<Core.Entities.Poll?> GetById(string id, CancellationToken ct)
    {
        return await _collection.FindAsync(x => x.PollId == ObjectId.Parse(id), cancellationToken: ct)
            .ContinueWith(task => task.Result.FirstOrDefaultAsync(cancellationToken: ct), ct)
            .Unwrap();
    }

    public async Task<List<Core.Entities.Poll>> GetAll(string userId, CancellationToken ct)
    {
        return await _collection.FindAsync(x => x.OwnerId == userId, cancellationToken: ct)
            .ContinueWith(task => task.Result.ToListAsync(cancellationToken: ct), ct).Unwrap();
    }

    public async Task<List<Core.Entities.Poll>> GetByFilter(Expression<Func<Core.Entities.Poll, bool>> filter, CancellationToken ct)
    {
        return await _collection.FindAsync(filter, cancellationToken: ct)
            .ContinueWith(task => task.Result.ToListAsync(cancellationToken: ct), ct).Unwrap();
    }

    public async Task<string> Add(Core.Entities.Poll entity, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(entity.OwnerId))
        {
            throw new Exception("У опроса не проставлен идентификатор создателя опроса");
        }

        await _collection.InsertOneAsync(entity, cancellationToken: ct);
        return entity.PollId.ToString();
    }

    public async Task<List<string>> AddAll(List<Core.Entities.Poll> entities, CancellationToken ct)
    {
        if (entities.Any(x => string.IsNullOrEmpty(x.OwnerId)))
        {
            throw new Exception("У элементов списка не проставлен идентификатор создателя опроса");
        }

        await _collection.InsertManyAsync(entities, cancellationToken: ct);
        return entities.Select(x => x.PollId.ToString()).ToList();
    }

    public async Task<Core.Entities.Poll> Update(Core.Entities.Poll entity, CancellationToken ct)
    {
        var storedEntity = await GetById(entity.PollId.ToString(), ct);
        if (storedEntity is null)
        {
            throw new Exception("Сущность не найдена в базе данных");
        }

        entity.Pages.AddRange(storedEntity.Pages);
        entity.DepartmentIds.AddRange(storedEntity.DepartmentIds);
        entity.EmployeeIds.AddRange(storedEntity.EmployeeIds);
        entity.PassedEmployees.AddRange(storedEntity.PassedEmployees);

        await _collection.ReplaceOneAsync(x => x.PollId == entity.PollId, entity, cancellationToken: ct);
        return entity;
    }

    public async Task Delete(string id, CancellationToken ct)
    {
        await _collection.DeleteOneAsync(x => x.PollId == ObjectId.Parse(id), cancellationToken: ct);
    }
}