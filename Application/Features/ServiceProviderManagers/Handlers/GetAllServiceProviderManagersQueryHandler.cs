using Application.Features.ServiceProviderManagers.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ServiceProviderManagers.Handlers;

public class GetAllServiceProviderManagersQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllServiceProviderManagersQuery, Result<ICollection<ServiceProviderManager>>>
{
    public async Task<Result<ICollection<ServiceProviderManager>>> Handle(GetAllServiceProviderManagersQuery request,
        CancellationToken cancellationToken)
    {
        var serviceProviderManagers =
            await applicationDbContext.ServiceProviderManagers.ToListAsync(cancellationToken: cancellationToken);
        
        return serviceProviderManagers;
    }
}