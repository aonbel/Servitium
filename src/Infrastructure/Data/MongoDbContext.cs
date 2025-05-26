using Domain.Interfaces;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetMongoCollection<T>(string name)
    {
        var collection = _database.GetCollection<T>(name);
        return collection;
    }

    public IMongoDatabase GetMongoDatabase()
    {
        return _database;
    }

    public async Task ExecuteTransactionAsync(Func<IClientSessionHandle, Task> transactionAction,
        CancellationToken cancellationToken)
    {
        using var session = await _client.StartSessionAsync(cancellationToken: cancellationToken);

        session.StartTransaction();

        try
        {
            await transactionAction(session);
            await session.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await session.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }
}