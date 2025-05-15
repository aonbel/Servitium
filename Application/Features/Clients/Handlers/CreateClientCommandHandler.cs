using Application.Features.Clients.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Infrastructure.Authorization;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Clients.Handlers;

public sealed class CreateClientCommandHandler(
    IApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager) : IRequestHandler<CreateClientCommand, Result<Client>>
{
    public async Task<Result<Client>> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var person = await applicationDbContext.Persons.SingleOrDefaultAsync(
            p => p.Id == request.PersonId,
            cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundById(request.PersonId);
        }
        
        var user = await userManager.FindByIdAsync(person.UserId);

        if (user is null)
        {
            return UserErrors.NotFoundById(person.UserId);
        }
        
        await userManager.AddToRoleAsync(user, ApplicationRoles.Client);
        
        if (await userManager.IsInRoleAsync(user, ApplicationRoles.Unauthenticated))
        {
            await userManager.RemoveFromRoleAsync(user, ApplicationRoles.Unauthenticated);
        }

        var client = new Client
        {
            PersonId = request.PersonId,
            Birthday = request.Birthday,
            Gender = request.Gender
        };

        await applicationDbContext.Clients.AddAsync(client, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return client;
    }
}