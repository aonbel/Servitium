using Application.Features.Specialists.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public class GetSpecialistsByServiceProviderIdQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetSpecialistsByServiceProviderIdQuery, Result<ICollection<Specialist>>>
{
    public async Task<Result<ICollection<Specialist>>> Handle(GetSpecialistsByServiceProviderIdQuery request, CancellationToken cancellationToken)
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

        return specialists;
    }
}