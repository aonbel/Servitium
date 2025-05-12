using Application.Features.Clients.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Clients.Handlers;

public sealed class GetClientByIdQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetClientByIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync( [ request.ClientId ], cancellationToken);

        if (client is null)
        {
            return new Error("ClientNotFound", "Client with given id does not exist");
        }
        
        return client;
    }
}