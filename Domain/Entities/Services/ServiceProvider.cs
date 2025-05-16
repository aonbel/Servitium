using Domain.Entities.Core;
using Domain.Entities.People;

namespace Domain.Entities.Services;

public class ServiceProvider : Entity
{
    public required string ShortName { get; set; }
    public required string Address { get; set; }
    public required Coordinates Coordinates { get; set; }
    public required TimeOnlySegment WorkTime { get; set; }
    public required ICollection<DayOfWeek> WorkDays { get; set; }
    public required ICollection<string> Contacts { get; set; }
}