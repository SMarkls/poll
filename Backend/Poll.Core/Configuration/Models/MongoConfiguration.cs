namespace Poll.Core.Configuration.Models;

public class MongoConfiguration
{
    public required string ConnectionString { get; init; }
    public required string Database { get; init; }
}