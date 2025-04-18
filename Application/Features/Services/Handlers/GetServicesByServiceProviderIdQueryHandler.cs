using Application.Features.Services.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

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
            return new Error("ServiceProviderNotFound",
                $"Service provider with given id {request.ServiceProviderId} does not exist");
        }
        
        return serviceProvider.Services.ToList();
    }
}