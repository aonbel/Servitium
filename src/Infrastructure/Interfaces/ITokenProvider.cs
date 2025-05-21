using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Interfaces;

public interface ITokenProvider
{
    public Task<string> GenerateAccessToken(IdentityUser user);
    
    public bool ValidateAccessToken(string accessToken);
    
    public string GenerateRefreshToken();
}