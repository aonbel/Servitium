using Application.Features.Users.Commands;
using Domain.Abstractions.RefreshToken;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Servitium.Infrastructure;

public class TokenAuthFilter(
    TokenHandler tokenHandler, 
    ITokenProvider tokenProvider,
    ISender sender) : IAsyncPageFilter
{
    public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (context.HttpContext.Request.Path.StartsWithSegments("/SignIn") ||
            context.HttpContext.Request.Path.StartsWithSegments("/SignUp"))
        {
            await next();
            return;
        }

        var accessToken = tokenHandler.GetAccessToken();

        if (accessToken is null)
        {
            context.Result = new RedirectToPageResult("/SignIn");
            return;
        }
        
        var isAccessTokenValid = tokenProvider.ValidateAccessToken(accessToken);

        if (!isAccessTokenValid)
        {
            var signInByTokenCommand = new SignInByTokenCommand(tokenHandler.GetRefreshToken()!);
            
            var result = await sender.Send(signInByTokenCommand);

            if (result.IsError)
            {
                tokenHandler.ClearTokens();
                context.Result = new RedirectToPageResult("/SignIn");
                return;
            }
            
            var responce = result.Value;

            tokenHandler.SetTokensIntoCookie(responce.AccessToken, responce.RefreshToken);
            
            var currentUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            
            context.Result = new RedirectToPageResult(currentUrl);
            
            return;
        }
        
        await next();
    }
}