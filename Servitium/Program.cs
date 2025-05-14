using Application;
using Application.Features.Users.Commands;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Servitium;
using Servitium.Infrastructure;
using Serilog;
using Servitium.Middleware;

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
    var token = context.Request.Cookies["AccessToken"];
    if (!string.IsNullOrEmpty(token))
    {
        Console.WriteLine($"AccessToken from cookie: {token}");
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }
    else
    {
        Console.WriteLine("No AccessToken cookie found");
    }
    await next();
});

app.UseAuthentication();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Response.StatusCode == 401 &&
        context.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
    {
        var sender = context.RequestServices.GetRequiredService<ISender>();
        var tokenHandler = context.RequestServices.GetRequiredService<TokenHandler>();

        var signInByTokenCommand = new SignInByTokenCommand(refreshToken);

        var commandResult = await sender.Send(signInByTokenCommand);

        if (commandResult.IsError)
        {
            tokenHandler.ClearTokens();

            context.Response.Redirect("/SignIn");
            return;
        }

        var responce = commandResult.Value;

        tokenHandler.SetTokensIntoCookie(responce.AccessToken, responce.RefreshToken);

        context.Response.Redirect(context.Request.GetEncodedUrl());
    }

    await next();
});

app.UseStaticFiles();

app.MapRazorPages();

await app.RunAsync();