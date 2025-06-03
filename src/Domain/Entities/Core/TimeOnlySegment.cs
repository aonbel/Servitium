namespace Domain.Entities.Core;

public sealed class TimeOnlySegment
{
    public TimeSpan Begin { get; init; }
    public TimeSpan End { get; init; }

    public TimeOnlySegment()
    {
    }

    public TimeOnlySegment(TimeSpan begin, TimeSpan end)
    {
        if (begin > end)
        {
            end += TimeSpan.FromDays(1);
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

    public bool Contains(TimeSpan time)
    {
        return time >= Begin && time <= End;
    }

    public override string ToString()
    {
        var formattedEnd = End;

        if (formattedEnd > TimeSpan.FromDays(1))
        {
            formattedEnd -= TimeSpan.FromDays(1);
        }
        
        return Begin + "-" + formattedEnd;
    }
}