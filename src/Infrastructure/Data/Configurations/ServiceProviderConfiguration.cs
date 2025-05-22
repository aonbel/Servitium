using Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.OwnsOne(s => s.WorkTime, nb =>
        {
            nb.Property(p => p.Begin).HasColumnType("time");    
            nb.Property(p => p.End).HasColumnType("time");
        });
    }
}