using System.Text;
using Domain.Interfaces;
using Infrastructure.Authentification;
using Infrastructure.Authorization;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Options.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SerializationPlugin;
using SerializationPlugin.Serializers;

namespace Infrastructure;

public static class Di
{
    public static IServiceCollection AddAuthenticationLogic(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<AuthenticationOptions>()
            .BindConfiguration("Jwt")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"]
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = 401;

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddHttpContextAccessor();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
            connectionString,
            b => b.MigrationsAssembly("Servitium")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
        
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<IdentityRole>();
        
        services.AddScoped<RoleManager<IdentityRole>>();
        services.AddScoped<UserManager<IdentityUser>>();
        
        services.AddHostedService<RoleSeedService>();
        services.AddHostedService<UserSeedService>();

        services.AddScoped<JsonSerializer>();
        services.AddScoped<TomlSerializer>();

        return services;
    }
}