using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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

    public async Task<string> AddPollPage(PollPage pollPage, string pollId, CancellationToken ct)
    {
        pollPage.PageId = ObjectId.GenerateNewId();
        pollPage.Questions.ForEach(x => x.QuestionId = ObjectId.GenerateNewId());
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId));
        var update = Builders<Core.Entities.Poll>.Update.Push(x => x.Pages, pollPage);
        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
        return pollPage.PageId.ToString();
    }

    public async Task RemovePollPage(string pollPageId, string pollId, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId));
        var delete =
            Builders<Core.Entities.Poll>.Update.PullFilter(x => x.Pages,
                Builders<PollPage>.Filter.Eq(x => x.PageId, ObjectId.Parse(pollPageId)));
        await _collection.UpdateOneAsync(filter, delete);
    }

    public async Task UpdateHeader(string pollPageId, string pollId, string newHeader, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == ObjectId.Parse(pollPageId)));
        var update = Builders<Core.Entities.Poll>.Update.Set(x => x.Pages.FirstMatchingElement().PageHeader, newHeader);
        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
    
    public async Task<PollPage?> GetPollPage(string pollPageId, string pollId, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages, p => p.PageId == ObjectId.Parse(pollPageId))
        );
    
        var projection = Builders<Core.Entities.Poll>.Projection.Expression(p =>
            p.Pages.FirstOrDefault(page => page.PageId == ObjectId.Parse(pollPageId))
        );
    
        return await _collection.Find(filter).Project(projection).FirstOrDefaultAsync(cancellationToken: ct);
    }

    public async Task DeleteQuestion(string pollPageId, string pollId, string questionId, CancellationToken ct)
    {
        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(x => x.Pages,
                p => p.PageId == ObjectId.Parse(pollPageId) &&
                     p.Questions.Any(x => x.QuestionId == ObjectId.Parse(questionId))));

        var delete = Builders<Core.Entities.Poll>.Update.PullFilter(
            x => x.Pages.FirstMatchingElement().Questions,
            q => q.QuestionId == ObjectId.Parse(questionId));

        await _collection.UpdateOneAsync(filter, delete, cancellationToken: ct);
    }

    public async Task EditQuestionText(string pollPageId, string pollId, string questionId, string newHeader, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(pollId) || string.IsNullOrEmpty(pollPageId) || string.IsNullOrEmpty(questionId))
            throw new ArgumentException("Идентификаторы не могут быть пустыми");

        var filter = Builders<Core.Entities.Poll>.Filter.And(
            Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId)),
            Builders<Core.Entities.Poll>.Filter.ElemMatch(
                x => x.Pages,
                page => page.PageId == ObjectId.Parse(pollPageId) &&
                        page.Questions.Any(question => question.QuestionId == ObjectId.Parse(questionId))
            )
        );

        var arrayFilters = new List<ArrayFilterDefinition<Core.Entities.Poll>>
        {
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("i._id",
                ObjectId.Parse(pollPageId))),
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("j._id",
                ObjectId.Parse(questionId)))
        };

        const string updateString = "Pages.$[i].Questions.$[j].QuestionText";
        var update = Builders<Core.Entities.Poll>.Update.Set(updateString, newHeader);

        var result = await _collection.UpdateOneAsync(filter, update,
            options: new UpdateOptions { ArrayFilters = arrayFilters }, cancellationToken: ct);

        if (result.ModifiedCount == 0)
        {
            throw new InvalidOperationException("Текст вопроса не был обновлен.");
        }
    }

    public async Task<string> AddQuestion(string pollPageId, string pollId, Question question, CancellationToken ct)
    {
        question.QuestionId = ObjectId.GenerateNewId();
        const string updateString = "Pages.$[i].Questions";
        var update = Builders<Core.Entities.Poll>.Update.Push(updateString, question);
        var arrayFilters = new List<ArrayFilterDefinition<Core.Entities.Poll>>
        {
            new BsonDocumentArrayFilterDefinition<Core.Entities.Poll>(new BsonDocument("i._id",
                ObjectId.Parse(pollPageId))),
        };

        var filter = Builders<Core.Entities.Poll>.Filter.Eq(x => x.PollId, ObjectId.Parse(pollId));
        var result =
            await _collection.UpdateOneAsync(filter, update, new UpdateOptions { ArrayFilters = arrayFilters }, ct);

        if (result.ModifiedCount == 0)
        {
            throw new InvalidOperationException("Вопрос не был добавлен");
        }

        return question.QuestionId.ToString();
    }
}
