using Infrastructure.Options.Authentication;
using Microsoft.Extensions.Options;

namespace Servitium.Infrastructure;

public sealed class TokenHandler(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthenticationOptions> authenticationOptions)
{
    public void SetTokensIntoCookie(string accessToken, string refreshToken)
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return;
        }

        var accessTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(authenticationOptions.Value.AccessTokenCookieExpirationInMinutes)
        };

        var refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(authenticationOptions.Value.RefreshTokenCookieExpirationInDays)
        };
        
        httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", accessToken, accessTokenCookieOptions);
        httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, refreshTokenCookieOptions);
    }
    
    public string? GetAccessToken()
    {
        return httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"];
    }

    public string? GetRefreshToken()
    {
        return httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];
    }
    
    public void ClearTokens()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return;
        }
        
        httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");
        httpContextAccessor.HttpContext.Response.Cookies.Delete("RefreshToken");
    }
}