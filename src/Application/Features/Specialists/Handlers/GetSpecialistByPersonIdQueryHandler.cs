using Application.Features.Specialists.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public sealed class GetSpecialistByPersonIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetSpecialistByPersonIdQuery, Result<Specialist>>
{
    public async Task<Result<Specialist>> Handle(GetSpecialistByPersonIdQuery request,
        CancellationToken cancellationToken)
    {
        var specialist =
            await applicationDbContext.Specialists.SingleOrDefaultAsync(s => s.PersonId == request.PersonId,
                cancellationToken);

        if (specialist is null)
        {
            return SpecialistErrors.NotFoundByPersonId(request.PersonId);
        }
        
        return specialist;
    }
}