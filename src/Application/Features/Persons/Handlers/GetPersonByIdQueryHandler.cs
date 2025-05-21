using Application.Features.Persons.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.Persons.Handlers;

public sealed class GetPersonByIdQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetPersonByIdQuery, Result<Person>>
{
    public async Task<Result<Person>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await applicationDbContext.Persons.FindAsync([ request.Id ], cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundById(request.Id);
        }
        
        return person;
    }
}