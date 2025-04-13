using Domain.Models.Entities.People;
using Domain.Models.Entities.Services;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; }
    
    DbSet<User> Users { get; }
    
    DbSet<Specialist> Specialists { get; }
    
    DbSet<ServiceProvider> ServiceProviders { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}