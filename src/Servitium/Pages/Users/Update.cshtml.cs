using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Servitium.Pages.Users;

public class Update : PageModel
{
    public InputModel Input { get; set; } = new();
    
    public class InputModel
    {
        
    }
    
    public void OnGet()
    {
        
    }
}