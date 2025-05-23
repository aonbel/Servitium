using Application.Features.Specialists.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public sealed class GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQueryHandler(
    IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery, Result<ICollection<Specialist>>>
{
    public async Task<Result<ICollection<Specialist>>> Handle(
        GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery request,
        CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var specialists = await applicationDbContext.Specialists
            .Where(s => s.ServiceProviderId == request.ServiceProviderId &&
                        s.WorkDays.Contains(request.Date.DayOfWeek) &&
                        s.ServiceIds.Contains(request.ServiceId))
            .ToListAsync(cancellationToken);

        return specialists;
    }
}