using Application.Features.Clients.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clients.Handlers;

public class GetClientByPersonIdQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetClientByPersonIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByPersonIdQuery request, CancellationToken cancellationToken)
    {
        var client =
            await applicationDbContext.Clients.FirstOrDefaultAsync(client => client.PersonId == request.PersonId,
                cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundByPersonId(request.PersonId);
        }
        
        return client;
    }
}