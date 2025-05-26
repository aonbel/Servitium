using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Infrastructure.Data.MongoRepositories;

public class MongoUserRepository(IMongoDbContext mongoDbContext) : IMongoRepository<IdentityUser>
{
    private const string CollectionName = nameof(IdentityUser);
    
    public async Task InsertOneAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var collection = mongoDbContext.GetMongoCollection<IdentityUser>(CollectionName);
        await collection.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public async Task InsertManyAsync(IEnumerable<IdentityUser> users, CancellationToken cancellationToken)
    {
        var collection = mongoDbContext.GetMongoCollection<IdentityUser>(CollectionName);
        await collection.InsertManyAsync(users, cancellationToken: cancellationToken);
    }

    public async Task UpdateOneAsync(FilterDefinition<IdentityUser> filter, UpdateDefinition<IdentityUser> update, CancellationToken cancellationToken)
    {
        var collection = mongoDbContext.GetMongoCollection<IdentityUser>(CollectionName);
        await collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
    }

    public async Task DeleteOneAsync(FilterDefinition<IdentityUser> filter, CancellationToken cancellationToken)
    {
        var collection = mongoDbContext.GetMongoCollection<IdentityUser>(CollectionName);
        await collection.DeleteOneAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<List<IdentityUser>> FindAsync(FilterDefinition<IdentityUser> filter, CancellationToken cancellationToken)
    {
        var collection = mongoDbContext.GetMongoCollection<IdentityUser>(CollectionName);
        return await collection.Find(filter).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<IdentityUser>> GetAllAsync(CancellationToken cancellationToken)
    {
        var collection = mongoDbContext.GetMongoCollection<IdentityUser>(CollectionName);
        return await collection.Find(_ => true).ToListAsync(cancellationToken: cancellationToken);
    }
}