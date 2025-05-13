using Application.Features.Clients.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clients.Handlers;

public sealed class CreateClientCommandHandler(IApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager) : IRequestHandler<CreateClientCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var userExists = await userManager.Users.AnyAsync(
            u => u.Id == request.UserId, 
            cancellationToken);
        
        if (!userExists)
        {
            return UserErrors.NotFoundById(request.UserId);
        }
        
        var client = new Client
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Birthday = request.Birthday,
            Gender = request.Gender,
            Phone = request.Phone
        };

        await applicationDbContext.Clients.AddAsync(client, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}