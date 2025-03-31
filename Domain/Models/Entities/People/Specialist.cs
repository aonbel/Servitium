using Domain.Models.Entities.Services;

namespace Domain.Models.Entities.People;

public class Specialist : Person
{
    public required decimal PricePerHour { get; set; }
    public required (TimeOnly begin, TimeOnly end) WorkTime { get; set; }
    public required DayOfWeek[] WorkDays { get; set; }
    public required string Contacts { get; set; }
    public required string Location { get; set; }
    public required Service[] Services { get; set; }
}