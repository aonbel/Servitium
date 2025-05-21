using Application.Features.Services.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Services.Handlers;

public sealed class GetServicesByServiceProviderIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServicesByServiceProviderIdQuery, Result<ICollection<Service>>>
{
    public async Task<Result<ICollection<Service>>> Handle(GetServicesByServiceProviderIdQuery request,
        CancellationToken cancellationToken)
    {
        var serviceProvider =
            await applicationDbContext.ServiceProviders.FindAsync([request.ServiceProviderId], cancellationToken);

        if (serviceProvider is null)
        {
            return ServiceProviderErrors.NotFoundById(request.ServiceProviderId);
        }

        var specialists = applicationDbContext.Specialists
            .Where(s => s.ServiceProviderId == request.ServiceProviderId);

        var serviceIds = specialists
            .SelectMany(specialist => specialist.ServiceIds)
            .Distinct();
        
        List<Service> services = [];
        
        foreach (var serviceId in serviceIds)
        {
            var service = await applicationDbContext.Services.FindAsync([serviceId], cancellationToken);

            if (service is null)
            {
                return ServiceErrors.NotFoundById(serviceId);
            }
            
            services.Add(service);
        }

        return services;
    }
}