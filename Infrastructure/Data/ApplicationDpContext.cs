using Application.Interfaces;
using Domain.Models.Entities.People;
using Domain.Models.Entities.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<Client> Clients { get; }
    public DbSet<User> Users { get; }
    public DbSet<Specialist> Specialists { get; }
    public DbSet<ServiceProvider> ServiceProviders { get; }
    public DbSet<ServiceProviderManager> ServiceProviderManagers { get; }
    public DbSet<HealthCertificateTemplate> HealthСertificateTemplates { get; }
    public DbSet<HealthCertificate> HealthСertificates { get; }
    public DbSet<Service> Services { get; }
    public DbSet<Appointment> Appointments { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}