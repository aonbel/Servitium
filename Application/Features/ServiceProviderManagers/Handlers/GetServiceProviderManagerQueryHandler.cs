using Application.Features.ServiceProviderManagers.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Handlers;

public sealed class GetServiceProviderManagerByIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServiceProviderManagerByIdQuery, Result<ServiceProviderManager>>
{
    public async Task<Result<ServiceProviderManager>> Handle(GetServiceProviderManagerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var serviceProviderManager =
            await applicationDbContext.ServiceProviderManagers.FindAsync([request.Id], cancellationToken);

        if (serviceProviderManager is null)
        {
            return ServiceProviderManagerErrors.NotFoundById(request.Id);
        }

        return serviceProviderManager;
    }
}