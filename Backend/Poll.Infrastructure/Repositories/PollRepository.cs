using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Poll.Core.Entities.Answers;
using Poll.Core.Exceptions;
using Poll.Core.Interfaces;
using Poll.Infrastructure.MongoConnection;

namespace Poll.Infrastructure.Repositories;

public class PollRepository : IPollRepository
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
        return await _collection.FindAsync(x => x.PollId == id, cancellationToken: ct)
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
        return entity.PollId;
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
        var storedEntity = await GetById(entity.PollId, ct);
        if (storedEntity is null)
        {
            _logger.LogError("Опрос с идентификатором {Id} не найден в базе данных", entity.PollId);
            throw new NotFoundException(entity.PollId, typeof(Core.Entities.Poll));
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
        await _collection.DeleteOneAsync(x => x.PollId == id, cancellationToken: ct);
    }

    public async Task<string> GetOwnerId(string pollId, CancellationToken ct)
    {
        var projection = Builders<Core.Entities.Poll>.Projection.Include(x => x.OwnerId);
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId);
        var result = await _collection.Find(filter)
            .Project(projection)
            .FirstOrDefaultAsync(cancellationToken: ct);

        if (result != null)
        {
            return result["OwnerId"].ToString() ?? String.Empty;
        }

        return string.Empty;
    }

    public async Task Complete(string pollId, string userId, CompletePollDto dto, CancellationToken ct)
    {
        var updates = new List<WriteModel<Core.Entities.Poll>>();
    
        // 1. Добавляем сотрудника в список прошедших опрос
        var passedEmployeesUpdate = Builders<Core.Entities.Poll>.Update
            .AddToSet(p => p.PassedEmployees, userId);
    
        updates.Add(new UpdateOneModel<Core.Entities.Poll>(
            Builders<Core.Entities.Poll>.Filter.Eq(p => p.PollId, pollId),
            passedEmployeesUpdate
        ));

        // 2. Обрабатываем ответы сотрудника
        foreach (var dtoPage in dto.Pages)
        {
            foreach (var dtoQuestion in dtoPage.Questions)
            {
                var questionId = ObjectId.Parse(dtoQuestion.QuestionId);
                var answerPath = $"Answers.{userId}.{questionId}";

                var answerUpdate = Builders<Core.Entities.Poll>.Update
                    .Set(answerPath, dtoQuestion.Value);

                updates.Add(new UpdateOneModel<Core.Entities.Poll>(
                    Builders<Core.Entities.Poll>.Filter.Eq(p => p.PollId, pollId),
                    answerUpdate
                ));
            }
        }

        await _collection.BulkWriteAsync(updates, cancellationToken: ct);
    }

    public Task PersistProgress(string pollId, string userId, CompletePollDto dto, CancellationToken ct) =>
        Task.CompletedTask;

    public Task<CompletePollDto?> GetProgress(string pollId, string userId, CancellationToken ct) =>
        Task.FromResult<CompletePollDto?>(null);

    public async Task<Dictionary<string, string>?> GetEdit(string pollId, string userId, CancellationToken ct)
    {
        var projection = Builders<Core.Entities.Poll>.Projection.Include(x => x.Answers[userId]);
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId);
        var result = await _collection.FindAsync(filter, new FindOptions<Core.Entities.Poll> { Projection = projection },
            cancellationToken: ct);
        if (result is null)
        {
            throw new AppException("Получить результат прохождения опроса не получилось");
        }

        var poll = await result.FirstOrDefaultAsync(cancellationToken: ct);
        if (poll is null)
        {
            throw new AppException("Получить результат прохождения опроса не получилось");
        }

        var dict = poll.Answers.FirstOrDefault(x => x.Key == userId).Value;
        return dict.ToDictionary(x => x.Key.ToString(), x => x.Value);
    }
}