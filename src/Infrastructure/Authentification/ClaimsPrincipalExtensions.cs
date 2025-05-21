using System.Security.Claims;

namespace Infrastructure.Authentification;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        var id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (id is null)
        {
            throw new ApplicationException("Cannot access user identifier claim.");
        }
        
        return id;
    }
}