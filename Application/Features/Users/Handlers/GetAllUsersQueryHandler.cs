using Application.Features.Users.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public class GetAllUsersQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllUsersQuery, Result<ICollection<User>>>
{
    public async Task<Result<ICollection<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await applicationDbContext.Users.ToListAsync(cancellationToken);
        
        return users;
    }
}