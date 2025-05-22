using System.Reflection;
using Servitium.Extensions;
using Servitium.Infrastructure;

namespace Servitium;

public static class Di
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpoints(Assembly.GetExecutingAssembly());
        
        services.AddScoped<TokenHandler>();
        services.AddScoped<RoleSelectionService>();
        
        return services;
    }
}