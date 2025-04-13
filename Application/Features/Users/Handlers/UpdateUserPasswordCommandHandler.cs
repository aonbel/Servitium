using Application.Features.Users.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public sealed class UpdateUserPasswordCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<UpdateUserPasswordCommand, Result>
{
    public async Task<Result> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var rowsUpdates = await applicationDbContext.Users
            .Where(user => user.Id == request.Id)
            .ExecuteUpdateAsync(
                propertyCalls => propertyCalls.SetProperty(user => user.Password, request.Password),
                cancellationToken);

        return rowsUpdates == 0 ? new Error("UserNotFound", "User with given id does not exist") : Result.Success();
    }
}