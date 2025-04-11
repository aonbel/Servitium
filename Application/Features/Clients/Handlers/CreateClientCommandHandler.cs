using Application.Features.Clients.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Clients.Handlers;

public class CreateClientCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<CreateClientCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = new Client
        {
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Birthday = request.Birthday,
            Gender = request.Gender,
            Phone = request.Phone
        };

        await applicationDbContext.Clients.AddAsync(client, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}