using Application.Features.ServiceProviderManagers.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ServiceProviderManagers.Handlers;

public sealed class CreateServiceProviderManagerCommandHandler(
    IApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager)
    : IRequestHandler<CreateServiceProviderManagerCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateServiceProviderManagerCommand request,
        CancellationToken cancellationToken)
    {
        var serviceProvider =
            await applicationDbContext.ServiceProviders.FindAsync([request.ServiceProviderId], cancellationToken);

        if (serviceProvider is null)
        {
            return ServiceProviderErrors.NotFoundById(request.ServiceProviderId);
        }

        var user = await applicationDbContext.Persons.SingleOrDefaultAsync(u => u.Id == request.PersonId, cancellationToken);

        if (user is null)
        {
            return PersonErrors.NotFoundById(request.PersonId);
        }

        var serviceProviderManager = new ServiceProviderManager
        {
            PersonId = request.PersonId,
            ServiceProviderId = request.ServiceProviderId
        };

        await applicationDbContext.ServiceProviderManagers.AddAsync(serviceProviderManager, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}