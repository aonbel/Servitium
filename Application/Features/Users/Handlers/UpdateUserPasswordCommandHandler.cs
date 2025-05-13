using Application.Features.Users.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Handlers;

public sealed class UpdateUserPasswordCommandHandler(
    UserManager<IdentityUser> userManager)
    : IRequestHandler<UpdateUserPasswordCommand, Result>
{
    public async Task<Result> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users.Where(u => u.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFoundById(request.Id);
        }

        user.PasswordHash = request.Password;
        
        await userManager.UpdateAsync(user);

        return Result.Success();
    }
}