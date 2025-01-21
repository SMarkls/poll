using MongoDB.Driver;

namespace Poll.Infrastructure.MongoConnection;

public interface IMongoCollectionFactory<T>
{
    IMongoCollection<T> GetCollection();
}