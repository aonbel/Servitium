using System.Text;
using Domain.Interfaces;
using Infrastructure.Authentification;
using Infrastructure.Authorization;
using Infrastructure.Data;
using Infrastructure.Data.MongoRepositories;
using Infrastructure.Interfaces;
using Infrastructure.Options.Authentication;
using Infrastructure.Serialization;
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
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("Default")
                                   ?? throw new InvalidOperationException(
                                       "Connection string 'Default' not found.");

            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Servitium"));
        });
        
        // TODO DELETE THAT SHIT
        
        services.AddSingleton<IMongoDbContext>(_ =>
        {
            var connectionString = configuration.GetConnectionString("MongoDb")
                                   ?? throw new InvalidOperationException(
                                       "Connection string 'MongoDb' not found.");
            var databaseName = configuration["MongoDb:DatabaseName"]
                               ?? throw new InvalidOperationException(
                                   "MongoDB database name 'MongoDb:DatabaseName' not found.");
            return new MongoDbContext(connectionString, databaseName);
        });

        services.AddScoped<IMongoRepository<IdentityUser>, MongoUserRepository>();
            
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<IdentityRole>();

        services.AddScoped<RoleManager<IdentityRole>>();
        services.AddScoped<UserManager<IdentityUser>>();

        services.AddHostedService<RoleSeedService>();
        services.AddHostedService<UserSeedService>();

        services.AddScoped<SerializationService>();

        return services;
    }
}