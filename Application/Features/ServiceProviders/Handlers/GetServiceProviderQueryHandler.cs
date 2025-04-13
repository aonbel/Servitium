using Application.Features.ServiceProviders.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Handlers;

public sealed class GetServiceProviderQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServiceProviderQuery, Result<ServiceProvider>>
{
    public async Task<Result<ServiceProvider>> Handle(GetServiceProviderQuery request, CancellationToken cancellationToken)
    {
        var serviceProvider = await applicationDbContext.ServiceProviders.FindAsync(
            [request.Id], 
            cancellationToken);

        if (serviceProvider is null)
        {
            return new Error("ServiceProviderNotFound", "Service provider with given id does not exist");
        }
        
        return serviceProvider;
    }
}