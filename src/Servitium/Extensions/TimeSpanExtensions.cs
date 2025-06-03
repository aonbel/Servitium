namespace Servitium.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan ToUtc(this TimeSpan timeSpan)
    {
        var dt = DateTime.Now.Date.Add(timeSpan);
        var dtUtc = dt.ToUniversalTime();
        return dtUtc - dtUtc.Date;
    }

    public static TimeSpan ToLocal(this TimeSpan timeSpan)
    {
        var dt = DateTime.Now.Date.Add(timeSpan);
        var dtLocal = dt.ToLocalTime();
        return dtLocal - dtLocal.Date;
    }
}