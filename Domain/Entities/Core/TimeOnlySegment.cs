namespace Domain.Entities.Core;

public sealed class TimeOnlySegment
{
    public required TimeOnly Begin { get; init; }
    public required TimeOnly End { get; init; }

    public TimeOnlySegment(TimeOnly begin, TimeOnly end)
    {
        if (begin > end)
        {
            throw new ArgumentException("The begin time cannot be greater than the end time."); 
        }
        
        Begin = begin;
        End = end;
    }

    public bool IsIntersecting(TimeOnlySegment other)
    {
        return Begin < other.End && End > other.Begin;
    }

    public bool Contains(TimeOnlySegment other)
    {
        return Begin <= other.Begin && other.End <= End;
    }
}