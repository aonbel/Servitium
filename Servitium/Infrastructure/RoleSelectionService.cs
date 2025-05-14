using Microsoft.AspNetCore.Identity;

namespace Servitium.Infrastructure;

public class RoleSelectionService(UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
{
    public async Task<bool> SetSelectedRole(string roleName)
    {
        var user = (await userManager.GetUserAsync(contextAccessor.HttpContext!.User))!;

        var roles = await userManager.GetRolesAsync(user);

        if (!roles.Contains(roleName))
        {
            return false;
        }
        
        contextAccessor.HttpContext.Response.Cookies.Append("SelectedRole", roleName);

        return true;
    }

    public async Task<string> GetSelectedRole()
    {
        var user = (await userManager.GetUserAsync(contextAccessor.HttpContext!.User))!;

        return contextAccessor.HttpContext.Request.Cookies["SelectedRole"] ??
               (await userManager.GetRolesAsync(user)).FirstOrDefault()!;
    }
}