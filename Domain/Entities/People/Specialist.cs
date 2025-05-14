using Domain.Entities.Core;
using Domain.Entities.Services;

namespace Domain.Entities.People;

public class Specialist : BaseEntity
{
    public required int PersonId { get; set; }
    public required decimal PricePerHour { get; set; }
    public required TimeOnlySegment WorkTime { get; set; }
    public required ICollection<DayOfWeek> WorkDays { get; set; }
    public required ICollection<string> Contacts { get; set; }
    public required string Location { get; set; }
    public required ICollection<Service> Services { get; set; } = [];
}