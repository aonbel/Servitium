using Application.Features.Users.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public sealed class CreateUserCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateUserCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await applicationDbContext.Users.AnyAsync(user => user.Username == request.Username, cancellationToken))
        {
            return new Error("InvalidUsername", "Username already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Password = request.Password,
            Role = request.Role
        };

        await applicationDbContext.Users.AddAsync(user, cancellationToken);

        var userId = await applicationDbContext.SaveChangesAsync(cancellationToken);

        return userId;
    }
}