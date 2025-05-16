using Domain.Abstractions.RefreshToken;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Authorization;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options), IApplicationDbContext
{
    public DbSet<Person> Persons { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Specialist> Specialists { get; set; }
    public DbSet<ServiceProvider> ServiceProviders { get; set; }
    public DbSet<ServiceProviderManager> ServiceProviderManagers { get; set; }
    public DbSet<HealthCertificateTemplate> HealthCertificateTemplates { get; set; }
    public DbSet<HealthCertificate> HealthCertificates { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}