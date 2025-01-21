using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Poll.Core.Configuration.Models;

namespace Poll.Infrastructure.MongoConnection;

public class MongoCollectionFactory<T> : IMongoCollectionFactory<T>
{
    private readonly MongoConfiguration _configuration;

    public MongoCollectionFactory(IOptions<MongoConfiguration> configuration)
    {
        _configuration = configuration.Value;
    }

    public IMongoCollection<T> GetCollection()
    {
        var client = new MongoClient(_configuration.ConnectionString);
        var database = client.GetDatabase(_configuration.Database);
        var collection =
            database.GetCollection<T>(typeof(T).Name);
        return collection;
    }
}