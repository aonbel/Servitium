using System.Security.Cryptography;
using Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Authentification;

public class UserSeedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await userManager.Users.AnyAsync(u => u.UserName == "admin", cancellationToken))
        {
            var newAdmin = new IdentityUser
            {
                UserName = "admin",
                PasswordHash = Convert.ToBase64String(RandomNumberGenerator.GetBytes(10))
            };
            
            await userManager.CreateAsync(newAdmin);
            
            await userManager.AddToRoleAsync(newAdmin, ApplicationRoles.Admin);
            await userManager.AddToRoleAsync(newAdmin, ApplicationRoles.Client);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}