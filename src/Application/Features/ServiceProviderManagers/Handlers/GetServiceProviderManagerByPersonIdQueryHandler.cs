using Application.Features.ServiceProviderManagers.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ServiceProviderManagers.Handlers;

public class GetServiceProviderManagerByPersonIdHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServiceProviderManagerByPersonIdQuery, Result<ServiceProviderManager>>
{
    public async Task<Result<ServiceProviderManager>> Handle(GetServiceProviderManagerByPersonIdQuery request,
        CancellationToken cancellationToken)
    {
        var serviceProviderManager =
            await applicationDbContext.ServiceProviderManagers.SingleOrDefaultAsync(
                spm => spm.PersonId == request.PersonId, cancellationToken: cancellationToken);

        if (serviceProviderManager is null)
        {
            return ServiceProviderManagerErrors.NotFoundByPersonId(request.PersonId);
        }
        
        return serviceProviderManager;
    }
}