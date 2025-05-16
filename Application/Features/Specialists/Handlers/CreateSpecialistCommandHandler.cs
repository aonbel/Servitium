using Application.Features.Specialists.Commands;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public sealed class CreateSpecialistCommandHandler(
    IApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager)
    : IRequestHandler<CreateSpecialistCommand, Result<Specialist>>
{
    public async Task<Result<Specialist>> Handle(CreateSpecialistCommand request, CancellationToken cancellationToken)
    {
        var person = await applicationDbContext.Persons.SingleOrDefaultAsync(
            p => p.Id == request.PersonId,
            cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundById(request.PersonId);
        }

        var serviceProvider = await applicationDbContext.ServiceProviders.SingleOrDefaultAsync(
            sp => sp.Id == request.ServiceProviderId,
            cancellationToken);

        if (serviceProvider is null)
        {
            return ServiceProviderErrors.NotFoundById(request.ServiceProviderId);
        }
        
        var user = await userManager.FindByIdAsync(person.UserId);

        if (user is null)
        {
            return UserErrors.NotFoundById(person.UserId);
        }
        
        if (await userManager.IsInRoleAsync(user, ApplicationRoles.Specialist))
        {
            return UserErrors.RoleAlreadyAssignedToUser(ApplicationRoles.Specialist);
        }
        
        await userManager.AddToRoleAsync(user, ApplicationRoles.Specialist);
        
        if (await userManager.IsInRoleAsync(user, ApplicationRoles.Unauthenticated))
        {
            await userManager.RemoveFromRoleAsync(user, ApplicationRoles.Unauthenticated);
        }

        var specialist = new Specialist
        {
            ServiceProviderId = request.ServiceProviderId,
            PersonId = request.PersonId,
            Contacts = request.Contacts,
            Location = request.Location,
            PricePerHour = request.PricePerHour,
            WorkTime = request.WorkTime,
            WorkDays = request.WorkDays,
            ServiceIds = []
        };

        await applicationDbContext.Specialists.AddAsync(specialist, cancellationToken);

        return specialist;
    }
}