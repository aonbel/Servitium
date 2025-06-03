using Application.Features.Users.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Handlers;

public class GetRolesByUserIdQueryHandler(UserManager<IdentityUser> userManager)
    : IRequestHandler<GetRolesByUserIdQuery, Result<ICollection<string>>>
{
    public async Task<Result<ICollection<string>>> Handle(GetRolesByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = userManager.Users.SingleOrDefault(u => u.Id == request.UserId);

        if (user is null)
        {
            return UserErrors.NotFoundById(request.UserId);
        }
        
        var roles = await userManager.GetRolesAsync(user);

        return roles.ToList();
    }
}