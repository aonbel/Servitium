namespace Servitium.Infrastructure.PagesConstants;

public static class DaysOfWeek
{
    public static readonly List<string> DaysOfWeekList = Enum.GetNames<DayOfWeek>().ToList();
    
    public static readonly int DaysOfWeekCount = DaysOfWeekList.Count;
}