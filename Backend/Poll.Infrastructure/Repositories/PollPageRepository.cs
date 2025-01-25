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
                Builders<PollPage>.Filter.Eq(x => x.PageId, new ObjectId(pollPageId)));
        await _collection.UpdateOneAsync(filter, delete);
    }

    public async Task UpdateHeader(string pollPageId, string pollId, string newHeader)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == new ObjectId(pollPageId)));
        var update = Builders<Core.Entities.Poll>.Update.Set(x => x.Pages[-1].PageHeader, newHeader);
        await _collection.UpdateOneAsync(filter, update);
    }
    
    public async Task<PollPage?> GetPollPage(string pollPageId, string pollId)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == new ObjectId(pollPageId))
        );
    
        var projection = Builders<Core.Entities.Poll>.Projection.Expression(p =>
            p.Pages.FirstOrDefault(page => page.PageId == new ObjectId(pollPageId))
        );
    
        return await _collection.Find(filter).Project(projection).FirstOrDefaultAsync();
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

    public async Task EditQuestionText(string pollId, string pollPageId, string questionId, string newHeader)
    {
        if (string.IsNullOrEmpty(pollId) || string.IsNullOrEmpty(pollPageId) || string.IsNullOrEmpty(questionId))
            throw new ArgumentException("PollId, PollPageId, and QuestionId cannot be null or empty");

        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, new ObjectId(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(
                x => x.Pages,
                page => page.PageId == new ObjectId(pollPageId) &&
                        page.Questions.Any(question => question.QuestionId == new ObjectId(questionId))
            )
        );

        var update = Builders<Core.Entities.Poll>.Update.Set(
            x => x.Pages[-1].Questions[-1].QuestionText, newHeader
        );

        var result = await _collection.UpdateOneAsync(filter, update);

        if (result.ModifiedCount == 0)
        {
            throw new InvalidOperationException("Question text could not be updated. Ensure the IDs are correct.");
        }
    }
}
