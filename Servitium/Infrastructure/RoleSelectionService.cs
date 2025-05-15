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
        if (!contextAccessor.HttpContext!.User.Identity!.IsAuthenticated)
        {
            return string.Empty;
        }
        
        var user = (await userManager.GetUserAsync(contextAccessor.HttpContext!.User))!;

        var selectedRole = contextAccessor.HttpContext.Request.Cookies["SelectedRole"];
        
        var roles = await userManager.GetRolesAsync(user);

        if (!roles.Any())
        {
            return string.Empty;
        }

        if (selectedRole is null)
        {
            return roles.First();
        }

        if (roles.Contains(selectedRole)) return selectedRole;
        
        ClearSelectedRole();
        return roles.First();

    }

    public void ClearSelectedRole()
    {
        contextAccessor.HttpContext!.Response.Cookies.Delete("SelectedRole");
    }
}