using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Servitium;
using Servitium.Infrastructure;
using Serilog;
using Servitium.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
    connectionString,
    b => b.MigrationsAssembly("Servitium")));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAllUsersQuery).Assembly);
});

builder.Services.AddAuthenticationLogic(builder.Configuration);

builder.Services.AddPresentation();

builder.Services.AddRazorPages();

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

/*app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();*/

await app.RunAsync();