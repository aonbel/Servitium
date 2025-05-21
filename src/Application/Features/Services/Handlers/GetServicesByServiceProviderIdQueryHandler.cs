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

        var specialists = await applicationDbContext.Specialists
            .Where(s => s.ServiceProviderId == request.ServiceProviderId)
            .ToListAsync(cancellationToken);
        
        // TODO

        throw new NotImplementedException();
    }
}