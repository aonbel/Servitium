using Domain.Entities.People;

namespace Domain.Interfaces;

public interface ITokenProvider
{
    public string GenerateAccessToken(User user);
    
    public bool ValidateAccessToken(string accessToken);
    
    public string GenerateRefreshToken();
}