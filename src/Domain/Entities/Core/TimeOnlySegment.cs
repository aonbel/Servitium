namespace Domain.Entities.Core;

public sealed class TimeOnlySegment
{
    public TimeOnly Begin { get; init; }
    public TimeOnly End { get; init; }

    public TimeOnlySegment()
    {
    }

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

    public bool Contains(TimeOnly time)
    {
        return time >= Begin && time <= End;
    }

    public override string ToString()
    {
        return Begin + "-" + End;
    }
}