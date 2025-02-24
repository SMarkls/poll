using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Poll.Core.Entities;
using Poll.Core.Exceptions;
using Poll.Core.Interfaces;
using Poll.Infrastructure.MongoConnection;

namespace Poll.Infrastructure.Repositories;

public class PollPageRepository : IPollPageRepository
{
    private readonly ILogger<PollPageRepository> _logger;
    private readonly IMongoCollection<Core.Entities.Poll> _collection;
    public PollPageRepository(IMongoCollectionFactory<Core.Entities.Poll> collectionFactory, ILogger<PollPageRepository> logger)
    {
        _logger = logger;
        _collection = collectionFactory.GetCollection();
    }

    public async Task<string> AddPollPage(string pollId, PollPage pollPage, CancellationToken ct)
    {
        pollPage.PageId = ObjectId.GenerateNewId().ToString();
        pollPage.Questions.ForEach(x => x.QuestionId = ObjectId.GenerateNewId().ToString());
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId);
        var update = Builders<Core.Entities.Poll>.Update.Push(x => x.Pages, pollPage);
        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
        return pollPage.PageId;
    }

    public async Task RemovePollPage(string pollId, string pollPageId, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId);
        var delete =
            Builders<Core.Entities.Poll>.Update.PullFilter(x => x.Pages,
                Builders<PollPage>.Filter.Eq(x => x.PageId, pollPageId));
        await _collection.UpdateOneAsync(filter, delete, cancellationToken: ct);
    }

    public async Task UpdateHeader(string pollId, string pollPageId, string newHeader, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == pollPageId));
        var update = Builders<Core.Entities.Poll>.Update.Set(x => x.Pages.FirstMatchingElement().PageHeader, newHeader);
        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
    
    public async Task<PollPage?> GetPollPage(string pollId, string pollPageId, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == pollPageId)
        );
    
        var projection = Builders<Core.Entities.Poll>.Projection.Expression(p =>
            p.Pages.FirstOrDefault(page => page.PageId == pollPageId)
        );
    
        return await _collection.Find(filter).Project(projection).FirstOrDefaultAsync(cancellationToken: ct);
    }

    public async Task DeleteQuestion(string pollId, string pollPageId, string questionId, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages,
                p => p.PageId == pollPageId &&
                     p.Questions.Any(x => x.QuestionId == questionId)));

        var delete = Builders<Core.Entities.Poll>.Update.PullFilter(
            x => x.Pages.FirstMatchingElement().Questions,
            q => q.QuestionId == questionId);

        await _collection.UpdateOneAsync(filter, delete, cancellationToken: ct);
    }

    public async Task UpdateQuestion(string pollId, string pollPageId, string questionId, Question entity, 
        CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(
                x => x.Pages,
                page => page.PageId == pollPageId && 
                        page.Questions.Any(q => q.QuestionId == questionId)
            )
        );

        var updateBuilder = Builders<Core.Entities.Poll>.Update;
        var updateDefinitions = new List<UpdateDefinition<Core.Entities.Poll>>();

        var bsonEntity = entity.ToBsonDocument();
        foreach (var field in bsonEntity)
        {
            if (field.Value.IsBsonNull) continue;

            updateDefinitions.Add(
                updateBuilder.Set($"Pages.$[page].Questions.$[question].{field.Name}", field.Value)
            );
        }

        if (updateDefinitions.Count == 0)
        {
            _logger.LogError("Нет полей для обновления");
            throw new AppException("Нет полей для обновления");
        }

        var combinedUpdate = updateBuilder.Combine(updateDefinitions);
        var arrayFilters = new List<ArrayFilterDefinition>
        {
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("page._id", pollPageId)),
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("question._id", questionId))
        };
        var options = new UpdateOptions { ArrayFilters = arrayFilters };
        var result = await _collection.UpdateOneAsync(
            filter, 
            combinedUpdate, 
            options, 
            ct
        );
        if (result.MatchedCount == 0)
        {
            _logger.LogError("В процессе обновления ни один документ не был обновлен");
            throw new AppException("Документ не найден");
        }
    }

    public async Task<string> AddQuestion(string pollId, string pollPageId, Question question, CancellationToken ct)
    {
        question.QuestionId = ObjectId.GenerateNewId().ToString();
        const string updateString = "Pages.$[i].Questions";
        var update = Builders<Core.Entities.Poll>.Update.Push(updateString, question);
        var arrayFilters = new List<ArrayFilterDefinition<Core.Entities.Poll>>
        {
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("i._id",
                ObjectId.Parse(pollPageId))),
        };

        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, pollId);
        var result =
            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters }, ct);

        if (result.ModifiedCount == 0)
        {
            _logger.LogError("Вопрос не было добавлен");
            throw new AppException("Вопрос не был добавлен");
        }

        return question.QuestionId;
    }
}
