namespace Domain.Entities.Core;

public sealed class DateOnlySegment
{
    public required DateOnly Begin { get; set; }
    public required DateOnly End { get; set; }
}