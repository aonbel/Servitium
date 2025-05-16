using Application.Features.ServiceProviderManagers.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Authorization;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ServiceProviderManagers.Handlers;

public sealed class CreateServiceProviderManagerCommandHandler(
    IApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager)
    : IRequestHandler<CreateServiceProviderManagerCommand, Result<ServiceProviderManager>>
{
    public async Task<Result<ServiceProviderManager>> Handle(CreateServiceProviderManagerCommand request,
        CancellationToken cancellationToken)
    {
        var serviceProvider =
            await applicationDbContext.ServiceProviders.FindAsync([request.ServiceProviderId], cancellationToken);

        if (serviceProvider is null)
        {
            return ServiceProviderErrors.NotFoundById(request.ServiceProviderId);
        }

        var person = await applicationDbContext.Persons.SingleOrDefaultAsync(p => p.Id == request.PersonId, cancellationToken);

        if (person is null)
        {
            return PersonErrors.NotFoundById(request.PersonId);
        }
        
        var user = await userManager.FindByIdAsync(person.UserId);

        if (user is null)
        {
            return UserErrors.NotFoundById(person.UserId);
        }

        if (await userManager.IsInRoleAsync(user, ApplicationRoles.Manager))
        {
            return UserErrors.RoleAlreadyAssignedToUser(ApplicationRoles.Manager);
        }

        await userManager.AddToRoleAsync(user, ApplicationRoles.Manager);
        
        if (await userManager.IsInRoleAsync(user, ApplicationRoles.Unauthenticated))
        {
            await userManager.RemoveFromRoleAsync(user, ApplicationRoles.Unauthenticated);
        }

        var serviceProviderManager = new ServiceProviderManager
        {
            PersonId = request.PersonId,
            ServiceProviderId = request.ServiceProviderId
        };

        await applicationDbContext.ServiceProviderManagers.AddAsync(serviceProviderManager, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        return serviceProviderManager;
    }
}