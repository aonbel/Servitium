using Application.Features.Users.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
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

        return rowsUpdates == 0 ? UserErrors.NotFoundById(request.Id) : Result.Success();
    }
}