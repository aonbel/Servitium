namespace Domain.Models.Entities.Core;

public sealed class TimeOnlySegment
{
    public required TimeOnly Begin { get; set; }
    public required TimeOnly End { get; set; }
}