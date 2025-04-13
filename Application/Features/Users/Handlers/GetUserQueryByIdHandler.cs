using Application.Features.Users.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Users.Handlers;

public sealed class GetUserQueryByIdHandler(IApplicationDbContext applicationDbContext) : 
    IRequestHandler<GetUserQueryById, Result<User>>
{
    public async Task<Result<User>> Handle(GetUserQueryById request, CancellationToken cancellationToken)
    {
        var user = await applicationDbContext.Users.FindAsync( [ request.Id ], cancellationToken);

        if (user is null)
        {
            return new Error("UserNotFound", $"User with given id {request.Id} does not exist");
        }

        return user;
    }
}