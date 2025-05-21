using Application.Features.Users.Queries;
using Domain.Abstractions.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public class GetAllUsersQueryHandler(UserManager<IdentityUser> userManager)
    : IRequestHandler<GetAllUsersQuery, Result<ICollection<IdentityUser>>>
{
    public async Task<Result<ICollection<IdentityUser>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);
        
        return users;
    }
}