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
            applicationDbContext.Persons.SingleOrDefault(p => p.UserId == request.UserId);

        if (person is null)
        {
            return PersonErrors.NotFoundByUserId(request.UserId);
        }

        var client =
            applicationDbContext.Clients.SingleOrDefault(c => c.PersonId == person.Id);

        if (client is null)
        {
            return ClientErrors.NotFoundByPersonId(person.Id ?? 0);
        }
        
        return client;
    }
}