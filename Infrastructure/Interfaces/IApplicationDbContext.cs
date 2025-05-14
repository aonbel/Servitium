using Domain.Abstractions.RefreshToken;
using Domain.Entities.People;
using Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Person> Persons { get; set; }
    
    DbSet<Client> Clients { get; }
    
    DbSet<Specialist> Specialists { get; }
    
    DbSet<ServiceProvider> ServiceProviders { get; }
    
    DbSet<ServiceProviderManager> ServiceProviderManagers { get; }
    
    DbSet<HealthCertificateTemplate> HealthCertificateTemplates { get; }
    
    DbSet<HealthCertificate> HealthCertificates { get; }
    
    DbSet<Service> Services { get; }
    
    DbSet<Appointment> Appointments { get; }
    
    DbSet<RefreshToken> RefreshTokens { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}