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
    
    public async Task<Core.Entities.Poll?> GetById(string id)
    {
        return await _collection.FindAsync(x => x.PollId == ObjectId.Parse(id))
            .ContinueWith(task => task.Result.FirstOrDefaultAsync())
            .Unwrap();
    }

    public async Task<List<Core.Entities.Poll>> GetAll(string userId)
    {
        return await _collection.FindAsync(x => x.OwnerId == userId).ContinueWith(task => task.Result.ToListAsync()).Unwrap();
    }

    public async Task<List<Core.Entities.Poll>> GetByFilter(Expression<Func<Core.Entities.Poll, bool>> filter)
    {
        return await _collection.FindAsync(filter).ContinueWith(task => task.Result.ToListAsync()).Unwrap();
    }

    public async Task<string> Add(Core.Entities.Poll entity)
    {
        if (string.IsNullOrEmpty(entity.OwnerId))
        {
            throw new Exception("У опроса не проставлен идентификатор создателя опроса");
        }

        await _collection.InsertOneAsync(entity);
        return entity.PollId.ToString();
    }

    public async Task<List<string>> AddAll(List<Core.Entities.Poll> entities)
    {
        if (entities.Any(x => string.IsNullOrEmpty(x.OwnerId)))
        {
            throw new Exception("У элеметов списка не проставлен идентификатор создателя опроса");
        }

        await _collection.InsertManyAsync(entities);
        return entities.Select(x => x.PollId.ToString()).ToList();
    }

    public async Task<Core.Entities.Poll> Update(Core.Entities.Poll entity)
    {
        var storedEntity = await GetById(entity.PollId.ToString());
        if (storedEntity is null)
        {
            throw new Exception("Сущность не найдена в базе данных");
        }

        entity.Pages.AddRange(storedEntity.Pages);
        entity.DepartmentIds.AddRange(storedEntity.DepartmentIds);
        entity.EmployeeIds.AddRange(storedEntity.EmployeeIds);
        entity.PassedEmployees.AddRange(storedEntity.PassedEmployees);

        await _collection.ReplaceOneAsync(x => x.PollId == entity.PollId, entity);
        return entity;
    }

    public async Task Delete(string id)
    {
        await _collection.DeleteOneAsync(x => x.PollId == ObjectId.Parse(id));
    }
}