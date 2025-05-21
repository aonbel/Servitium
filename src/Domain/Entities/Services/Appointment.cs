using Domain.Entities.Core;
using Domain.Entities.People;

namespace Domain.Entities.Services;

public class Appointment : BaseEntity
{
    public required int ClientId { get; set; }
    public required int SpecialistId { get; set; }
    public required int ServiceId { get; set; }
    public required DateOnly Date { get; set; }
    public required TimeOnlySegment TimeSegment { get; set; }
}