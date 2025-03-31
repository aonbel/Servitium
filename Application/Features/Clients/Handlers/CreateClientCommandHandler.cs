using Application.Features.Clients.Commands;
using MediatR;

namespace Application.Features.Clients.Handlers;

public class CreateClientCommandHandler() : IRequestHandler<CreateClientCommand, int>
{
    public async Task<int> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}