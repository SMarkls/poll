using MongoDB.Bson;
using MongoDB.Driver;
using Poll.Core.Entities.ViewRules;
using Poll.Core.Interfaces;
using Poll.Infrastructure.MongoConnection;

namespace Poll.Infrastructure.Repositories;

public class RuleRepository : IRuleRepository
{
    private readonly IMongoCollection<Core.Entities.Poll> _collection;

    public RuleRepository(IMongoCollectionFactory<Core.Entities.Poll> collectionFactory)
    {
        _collection = collectionFactory.GetCollection();
    }

    public async Task<string> CreateOrUpdateRule(ViewRule rule, string pollId, string pollPageId, string questionId,
        CancellationToken ct)
    {
        if (rule.RuleId == ObjectId.Empty)
        {
            rule.RuleId = ObjectId.GenerateNewId();
        }

        var filters = Builders<Core.Entities.Poll>.Filter;
        var filter = filters.And(
            filters.Eq(x => x.PollId, ObjectId.Parse(pollId)),
            filters.ElemMatch(x => x.Pages, x => x.PageId == ObjectId.Parse(pollPageId)));

        var update = Builders<Core.Entities.Poll>.Update.Set("Pages.$[i].Questions.$[j].ViewRule", rule);
        var arrayFilters = new List<ArrayFilterDefinition<Core.Entities.Poll>>
        {
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("i._id",
                ObjectId.Parse(pollPageId))),
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("j._id",
                ObjectId.Parse(questionId)))
        };

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters },
            cancellationToken: ct);

        return rule.RuleId.ToString();
    }

    public async Task DeleteRule(string pollId, string pollPageId, string questionId, CancellationToken ct)
    {     
        var filters = Builders<Core.Entities.Poll>.Filter;
        var filter = filters.And(
            filters.Eq(x => x.PollId, ObjectId.Parse(pollId)),
            filters.ElemMatch(x => x.Pages, x => x.PageId == ObjectId.Parse(pollPageId)));

        var update = Builders<Core.Entities.Poll>.Update.Unset("Pages.$[i].Questions.$[j].ViewRule");
        var arrayFilters = new List<ArrayFilterDefinition<Core.Entities.Poll>>
        {
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("i._id",
                ObjectId.Parse(pollPageId))),
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("j._id",
                ObjectId.Parse(questionId)))
        };

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters },
            cancellationToken: ct);
    }
}