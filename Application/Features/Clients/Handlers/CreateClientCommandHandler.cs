using Application.Features.Clients.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clients.Handlers;

public class CreateClientCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<CreateClientCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var userExists = await applicationDbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            return new Error("UserNotFound", "User with given id was not found");
        }
        
        var client = new Client
        {
            UserId = request.UserId,
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