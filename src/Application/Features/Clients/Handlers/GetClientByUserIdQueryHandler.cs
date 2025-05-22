using Application.Features.Clients.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clients.Handlers;

public class GetClientByUserIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetClientByUserIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByUserIdQuery request, CancellationToken cancellationToken)
    {
        var person =
            await applicationDbContext.Persons.SingleOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundByUserId(request.UserId);
        }

        var client =
            await applicationDbContext.Clients.SingleOrDefaultAsync(c => c.PersonId == person.Id, cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundByPersonId(person.Id ?? 0);
        }
        
        return client;
    }
}