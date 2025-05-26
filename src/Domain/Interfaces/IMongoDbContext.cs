using MongoDB.Driver;

namespace Domain.Interfaces;

public interface IMongoDbContext
{
    IMongoCollection<T> GetMongoCollection<T>(string name);

    IMongoDatabase GetMongoDatabase();

    Task ExecuteTransactionAsync(
        Func<IClientSessionHandle, Task> transactionAction,
        CancellationToken cancellationToken);
}