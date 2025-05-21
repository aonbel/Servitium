using Application.Features.Users.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public sealed class GetUserQueryByIdHandler(UserManager<IdentityUser> userManager) : 
    IRequestHandler<GetUserQueryById, Result<IdentityUser>>
{
    public async Task<Result<IdentityUser>> Handle(GetUserQueryById request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFoundById(request.Id);
        }

        return user;
    }
}