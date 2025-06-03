using Domain.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.OwnsOne(a => a.TimeSegment, nb =>
        {
            nb.Property(p => p.Begin)
                .HasColumnType("time");

            nb.Property(p => p.End)
                .HasColumnType("time");
        });
    }
}