using Application.Features.Clients.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Clients.Handlers;

public class GetClientQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetClientQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientQuery request, CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync( [ request.Id ], cancellationToken);

        if (client is null)
        {
            return new Error("404", "Client not found");
        }
        
        return client;
    }
}