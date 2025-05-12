using Domain.Entities.Core;
using Domain.Entities.People;

namespace Domain.Entities.Services;

public class Appointment : BaseEntity
{
    public required Client Client { get; set; }
    public required Specialist Specialist { get; set; }
    public required Service Service { get; set; }
    
    public required DateOnly Date { get; set; }
    public required TimeOnlySegment TimeSegment { get; set; }
}