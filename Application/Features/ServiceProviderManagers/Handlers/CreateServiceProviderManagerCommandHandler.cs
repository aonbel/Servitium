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
            return new Error("ServiceProviderNotFound",
                $"Service provider with given id {request.ServiceProviderId} does not exist");
        }

        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFoundById(request.UserId);
        }

        var serviceProviderManager = new ServiceProviderManager
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            ServiceProvider = serviceProvider
        };

        await applicationDbContext.ServiceProviderManagers.AddAsync(serviceProviderManager, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}