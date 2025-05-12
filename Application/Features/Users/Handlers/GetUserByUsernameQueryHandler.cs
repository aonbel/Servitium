using Application.Features.Users.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public sealed class GetUserByUsernameQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetUserByUsernameQuery, Result<User>>
{
    public async Task<Result<User>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await applicationDbContext.Users.FirstOrDefaultAsync(
            user => user.Username == request.Username,
            cancellationToken);

        if (user is null)
        {
            return new Error("UserNotFound", "User with given username does not exist");
        }

        return user;
    }
}