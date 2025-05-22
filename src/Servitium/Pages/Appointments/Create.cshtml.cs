using Domain.Entities.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Servitium.Pages.Appointments;

public class Create(ISender sender) : PageModel
{
    public const int DaysOffset = 7;
    
    public int ServiceId { get; set; }
    
    public List<SelectListItem> DaysSelectList { get; set; } = [];

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public DateOnly SelectedDate { get; set; }

        public int SelectedServiceProviderId { get; set; }

        public int SelectedSpecialistId { get; set; }

        public TimeOnlySegment SelectedTimeOnlySegment { get; set; } = new();
    }

    public IActionResult OnGet(int id, DateTime fromDate)
    {
        ServiceId = id;
        
        var today = DateOnly.FromDateTime(DateTime.Now);

        for (var dayOffset = 0; dayOffset < DaysOffset; dayOffset++)
        {
            var currentDate = today.AddDays(dayOffset);
            
            DaysSelectList.Add(new SelectListItem(currentDate.ToString("MM/dd/yyyy"),
                currentDate.ToString("MM/dd/yyyy")));
        }
        
        return Page();
    }
}