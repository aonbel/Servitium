using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Interfaces;
using Infrastructure.Options.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentification;

public sealed class TokenProvider(
    IOptions<AuthenticationOptions> authOptions,
    IServiceScopeFactory scopeFactory) : ITokenProvider
{
    public async Task<string> GenerateAccessToken(IdentityUser user)
    {
        var secretKey = authOptions.Value.Secret;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        ]);
        
        var scope = scopeFactory.CreateScope();
        
        var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>()!; 
        
        var roles = await userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(authOptions.Value.AccessTokenExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = authOptions.Value.Issuer,
            Audience = authOptions.Value.Audience,
        };
        
        var handler = new JsonWebTokenHandler();
        
        var token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public bool ValidateAccessToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(authOptions.Value.Secret);

        try
        {
            tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = authOptions.Value.Issuer,
                ValidateAudience = true,
                ValidAudience = authOptions.Value.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(authOptions.Value.RefreshTokenSizeInBytes));
    }
}