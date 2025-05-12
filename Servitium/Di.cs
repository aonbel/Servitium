using Servitium.Infrastructure;

namespace Servitium;

public static class Di
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddScoped<TokenHandler>();
        
        return services;
    }
}