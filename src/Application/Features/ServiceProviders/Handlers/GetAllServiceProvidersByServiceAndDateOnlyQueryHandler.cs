using Application.Features.ServiceProviders.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.ServiceProviders.Handlers;

public class GetAllServiceProvidersByServiceAndDateOnlyQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllServiceProvidersByServiceAndDateOnlyQuery, Result<ICollection<ServiceProvider>>>
{
    public async Task<Result<ICollection<ServiceProvider>>> Handle(
        GetAllServiceProvidersByServiceAndDateOnlyQuery request, CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var serviceProvidersIdsThatCanProvideService = applicationDbContext.Specialists
            .Where(s => s.ServiceIds.Contains(request.ServiceId) && s.WorkDays.Contains(request.Date.DayOfWeek))
            .Select(s => s.ServiceProviderId)
            .Distinct()
            .ToList();

        List<ServiceProvider> serviceProviders = [];

        foreach (var serviceProviderId in serviceProvidersIdsThatCanProvideService)
        {
            var serviceProvider =
                await applicationDbContext.ServiceProviders.FindAsync([serviceProviderId], cancellationToken);

            if (serviceProvider is null)
            {
                return ServiceErrors.NotFoundById(serviceProviderId);
            }

            serviceProviders.Add(serviceProvider);
        }

        return serviceProviders;
    }
}