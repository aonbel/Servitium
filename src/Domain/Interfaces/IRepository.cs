using MongoDB.Driver;

namespace Domain.Interfaces;

public interface IRepository<T>
{
    Task InsertOneAsync(T product, CancellationToken cancellationToken);
    Task InsertManyAsync(IEnumerable<T> products, CancellationToken cancellationToken);
    Task UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken);
    Task DeleteOneAsync(FilterDefinition<T> filter, CancellationToken cancellationToken);
    Task<List<T>> FindAsync(FilterDefinition<T> filter, CancellationToken cancellationToken);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken);
}