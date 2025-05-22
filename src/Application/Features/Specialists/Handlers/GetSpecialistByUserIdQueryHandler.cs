using Application.Features.Specialists.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public class GetSpecialistByUserIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetSpecialistByUserIdQuery, Result<Specialist>>
{
    public async Task<Result<Specialist>> Handle(GetSpecialistByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var person = await applicationDbContext.Persons.SingleOrDefaultAsync(p => p.UserId == request.UserId,
            cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundByUserId(request.UserId);
        }

        var specialist =
            await applicationDbContext.Specialists.SingleOrDefaultAsync(s => s.PersonId == person.Id,
                cancellationToken);

        if (specialist is null)
        {
            return SpecialistErrors.NotFoundByPersonId(person.Id ?? 0);
        }

        return specialist;
    }
}