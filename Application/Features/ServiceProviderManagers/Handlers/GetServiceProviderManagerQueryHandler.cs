using Application.Features.ServiceProviderManagers.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Handlers;

public sealed class GetServiceProviderManagerQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetServiceProviderManagerQuery, Result<ServiceProviderManager>>
{
    public async Task<Result<ServiceProviderManager>> Handle(GetServiceProviderManagerQuery request,
        CancellationToken cancellationToken)
    {
        var serviceProviderManager =
            await applicationDbContext.ServiceProviderManagers.FindAsync([request.UserId], cancellationToken);

        if (serviceProviderManager is null)
        {
            return new Error("UserNotFound", $"User with given id {request.UserId} does not exist");
        }

        return serviceProviderManager;
    }
}