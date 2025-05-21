using Application;
using Application.Features.Users.Commands;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Servitium;
using Servitium.Infrastructure;
using Serilog;
using Servitium.Middleware;
using Servitium.Pages;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddAuthenticationLogic(builder.Configuration)
    .AddApplication()
    .AddPresentation()
    .AddRazorPages();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseRouting();

app.Use(async (context, next) =>
{
    Console.WriteLine($"{context.Request.Path}");
    var accessToken = context.Request.Cookies["AccessToken"];
    if (!string.IsNullOrEmpty(accessToken))
    {
        context.Request.Headers.Append("Authorization", $"Bearer {accessToken}");
    }

    await next();
});

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var gotRefreshToken = context.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken);
    var gotAccessToken = context.Request.Cookies.TryGetValue("AccessToken", out _);

    if (gotRefreshToken && (!gotAccessToken || context.Response.StatusCode == 401))
    {
        var sender = context.RequestServices.GetRequiredService<ISender>();
        var tokenHandler = context.RequestServices.GetRequiredService<TokenHandler>();
        
        var signInByTokenCommand = new SignInByTokenCommand(refreshToken!);

        var commandResult = await sender.Send(signInByTokenCommand);
        
        if (commandResult.IsError)
        {
            tokenHandler.ClearTokens();
        }
        else
        {
            var responce = commandResult.Value;

            tokenHandler.SetTokensIntoCookie(responce.AccessToken, responce.RefreshToken);
        }
        
        context.Response.Redirect(context.Request.GetEncodedUrl());
    }

    await next();
});

app.UseStaticFiles();

app.MapRazorPages();

await app.RunAsync();