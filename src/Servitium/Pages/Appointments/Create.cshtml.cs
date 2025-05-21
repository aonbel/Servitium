using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Servitium.Pages.Appointments;

public class Create : PageModel
{
    public SelectList ServiceProviderSelectList { get; set; }
    
    public SelectList SpecialistSelectList { get; set; }
    
    public SelectList DateSelectList { get; set; }

    public class InputModel
    {
        public int? ServiceProviderId { get; set; }
        
        public int? SpecialistId { get; set; }
        
        public DateOnly? SelectedDate { get; set; }
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }
}