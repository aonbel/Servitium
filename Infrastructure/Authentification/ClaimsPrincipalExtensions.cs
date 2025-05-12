using System.Security.Claims;

namespace Infrastructure.Authentification;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        return int.TryParse(id, out var result) ? result : throw new ApplicationException("Cannot access user id");
    }
}