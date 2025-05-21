using Application.Features.Clients.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;

namespace Application.Features.Clients.Handlers;

public sealed class GetClientByIdQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetClientByIdQuery, Result<Client>>
{
    public async Task<Result<Client>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync( [ request.ClientId ], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }
        
        return client;
    }
}