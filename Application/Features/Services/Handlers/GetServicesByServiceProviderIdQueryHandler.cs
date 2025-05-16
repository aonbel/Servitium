using Application.Features.Services.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
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
            return ServiceProviderErrors.NotFoundById(request.ServiceProviderId);
        }

        // TODO
        
        return serviceProvider.Services.ToList();
    }
}