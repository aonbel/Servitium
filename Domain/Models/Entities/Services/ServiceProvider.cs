using Domain.Models.Entities.Core;
using Domain.Models.Entities.People;

namespace Domain.Models.Entities.Services;

public class ServiceProvider : Entity
{
    public required string ShortName { get; set; }
    public required string Address { get; set; }
    public required Coordinates Coordinates { get; set; }
    public required TimeOnlySegment WorkTime { get; set; }
    public required ICollection<DayOfWeek> WorkDays { get; set; }
    public required ICollection<string> Contacts { get; set; }
    public required ICollection<Specialist> Specialists { get; set; } = [];
    public required ICollection<Service> Services { get; set; } = [];
}