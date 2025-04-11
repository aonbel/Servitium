using Application.Features.Users.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public class GetUserByUsernameQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetUserByUsernameQuery, Result<User>>
{
    public async Task<Result<User>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await applicationDbContext.Users.FirstOrDefaultAsync(
            user => user.Username == request.Username,
            cancellationToken);

        if (user is null)
        {
            return new Error("UserNotFound", "User with given username was not found");
        }

        return user;
    }
}