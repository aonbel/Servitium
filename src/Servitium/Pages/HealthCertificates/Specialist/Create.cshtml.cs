using Domain.Entities.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.HealthCertificates.Specialist;

public class Create : PageModel
{
    public List<HealthCertificateTemplate> Data { get; set; } = [];

    [BindProperty] public InputModel Input { get; set; } = new();
    
    public class InputModel
    {
        public List<string> Descriprions { get; set; } = [];
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        return Page();
    }
}