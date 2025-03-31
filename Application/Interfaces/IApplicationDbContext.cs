using Domain.Models.Entities.People;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}