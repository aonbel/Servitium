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
    .AddAuthenticationLogic(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddPresentation()
    .AddRazorPages();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["AccessToken"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
    }

    await next();
});

//app.UseHttpsRedirection();
app.UseRouting();

app.UseStaticFiles();

app.UseAuthentication();

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

app.UseMiddleware<RequestContextLoggingMiddleware>();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.UseAuthentication();

app.MapRazorPages();

await app.RunAsync();