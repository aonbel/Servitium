using Application.Features.Users.Queries;
using Domain.Abstractions.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public sealed class GetUserByUsernameQueryHandler(UserManager<IdentityUser> userManager) :
    IRequestHandler<GetUserByUsernameQuery, Result<IdentityUser>>
{
    public async Task<Result<IdentityUser>> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.UserName == request.Username, cancellationToken);

        if (user is null)
        {
            return new Error("UserNotFound", "User with given username does not exist");
        }

        return user;
    }
}