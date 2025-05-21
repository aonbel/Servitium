using Microsoft.AspNetCore.Mvc.Rendering;

namespace Servitium.Infrastructure.PagesConstants;

public static class DaysOfWeek
{
    public static readonly List<string> DaysOfWeekList = Enum.GetNames<DayOfWeek>().ToList();

    public static readonly List<SelectListItem> DaysOfWeekSelectList = Enum
        .GetValues<DayOfWeek>()
        .Select(d => new SelectListItem
        {
            Text = d.ToString(),
            Value = ((int)d).ToString()
        })
        .ToList();

    public static readonly int DaysOfWeekCount = DaysOfWeekList.Count;
}