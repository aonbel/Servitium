using Domain.Models.Entities.Core;
using Domain.Models.Entities.People;

namespace Domain.Models.Entities.Services;

public class ServiceProvider : Entity
{
    public required string ShortName { get; set; }
    public required string Address { get; set; }
    public required (float longitude, float latitude) Coordinates { get; set; }
    public required (TimeOnly begin, TimeOnly end) WorkTime { get; set; }
    public required DayOfWeek[] WorkDays { get; set; }
    public required string Contacts { get; set; }
    public required Specialist[] Specialists { get; set; }
    public required Service[] Services { get; set; }
}