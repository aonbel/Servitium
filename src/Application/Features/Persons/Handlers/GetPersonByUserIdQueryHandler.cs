using Application.Features.Persons.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Persons.Handlers;

public class GetPersonByUserIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetPersonByUserIdQuery, Result<Person>>
{
    public async Task<Result<Person>> Handle(GetPersonByUserIdQuery request, CancellationToken cancellationToken)
    {
        var person =
            await applicationDbContext.Persons.SingleOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundByUserId(request.UserId);
        }
        
        return person;
    }
}