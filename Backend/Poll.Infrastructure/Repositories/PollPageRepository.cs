using MongoDB.Bson;
using MongoDB.Driver;
using Poll.Core.Entities;
using Poll.Core.Interfaces;
using Poll.Infrastructure.MongoConnection;

namespace Poll.Infrastructure.Repositories;

public class PollPageRepository : IPollPageRepository
{
    private readonly IMongoCollection<Core.Entities.Poll> _collection;
    public PollPageRepository(IMongoCollectionFactory<Core.Entities.Poll> collectionFactory)
    {
        _collection = collectionFactory.GetCollection();
    }

    public async Task<string> AddPollPage(PollPage pollPage, string pollId)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId));
        var update = Builders<Core.Entities.Poll>.Update.Push(x => x.Pages, pollPage);
        await _collection.UpdateOneAsync(filter, update);
        return pollPage.PageId.ToString();
    }

    public async Task RemovePollPage(string pollPageId, string pollId)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId));
        var delete =
            Builders<Core.Entities.Poll>.Update.PullFilter(x => x.Pages,
                Builders<PollPage>.Filter.Eq(x => x.PageId, new ObjectId(pollId)));
        await _collection.UpdateOneAsync(filter, delete);
    }

    public async Task UpdateHeader(string pollPageId, string pollId, string newHeader)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == new ObjectId(pollPageId)));
        var update = Builders<Core.Entities.Poll>.Update.Set("Pages.$.PageHeader", newHeader);
        await _collection.UpdateOneAsync(filter, update);
    }

    public Task<PollPage?> GetPollPage(string pollPageId, string pollId)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == new ObjectId(pollPageId)));
        return _collection.FindAsync(filter).ContinueWith(x =>
            x.Result.FirstOrDefaultAsync()
                .ContinueWith(x => x.Result.Pages.FirstOrDefault(x => x.PageId == new ObjectId(pollPageId)))).Unwrap();
    }

    public Task DeleteQuestion(string pollPageId, string pollId, string questionId)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == new ObjectId(pollPageId)));

        var delete = Builders<Core.Entities.Poll>.Update.PullFilter(
            x => x.Pages,
            Builders<PollPage>.Filter.Eq(x => x.PageId, new ObjectId(pollPageId)));
    }
}