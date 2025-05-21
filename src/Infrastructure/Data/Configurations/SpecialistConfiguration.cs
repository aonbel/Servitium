using Domain.Entities.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class SpecialistConfiguration : IEntityTypeConfiguration<Specialist>
{
    public void Configure(EntityTypeBuilder<Specialist> builder)
    {
        builder.OwnsOne(s => s.WorkTime, nb =>
        {
            nb.Property(p => p.Begin).HasColumnType("time");    
            nb.Property(p => p.End).HasColumnType("time");
        });
    }
}