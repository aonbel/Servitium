using Application.Features.Clients.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Clients.Handlers;

public sealed class GetClientByIdQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetClientByIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync( [ request.Id ], cancellationToken);

        if (client is null)
        {
            return new Error("ClientNotFound", "Client with given id does not exist");
        }
        
        return client;
    }
}