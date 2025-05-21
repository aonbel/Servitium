using System.ComponentModel.DataAnnotations;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Servitium.Infrastructure;

namespace Servitium.Pages.Profile;

public class Manage(UserManager<IdentityUser> userManager, RoleSelectionService roleSelectionService) : PageModel
{
    public SelectList? RoleList { get; set; } = new(ApplicationRoles.AllRoles);
    
    [BindProperty]
    public InputModel Input { get; set; } = new();
    
    public class InputModel
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    };
    
    public async Task<IActionResult> OnGetAsync()
    {
        var user = (await userManager.GetUserAsync(User))!;
        var roles = await userManager.GetRolesAsync(user);
        
        RoleList = new SelectList(roles);
        
        return Page();  
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {   
        returnUrl ??= Url.Content(Routes.Index);

        if (!await roleSelectionService.SetSelectedRole(Input.Role))
        {
            ModelState.AddModelError("Role", "Invalid role");
            return Page();
        }
        
        return LocalRedirect(returnUrl);
    }
}