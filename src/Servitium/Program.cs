using System.Net;
using System.Text.Json;
using Application;
using Application.Features.Users.Commands;
using Infrastructure;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Servitium;
using Servitium.Infrastructure;
using Serilog;
using Servitium.Extensions;
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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapEndpoints();

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

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error.",
            Detail = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.UseStaticFiles();

app.MapRazorPages();

await app.RunAsync();