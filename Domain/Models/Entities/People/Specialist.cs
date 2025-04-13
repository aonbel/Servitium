using Domain.Models.Entities.Core;
using Domain.Models.Entities.Services;

namespace Domain.Models.Entities.People;

public class Specialist : Person
{
    public required decimal PricePerHour { get; set; }
    public required TimeOnlySegment WorkTime { get; set; }
    public required ICollection<DayOfWeek> WorkDays { get; set; }
    public required ICollection<string> Contacts { get; set; }
    public required string Location { get; set; }
    public required ICollection<Service> Services { get; set; } = [];
}