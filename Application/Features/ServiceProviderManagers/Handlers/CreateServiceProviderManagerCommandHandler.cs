using Application.Features.ServiceProviderManagers.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Handlers;

public sealed class CreateServiceProviderManagerCommandHandler(IApplicationDbContext applicationDbContext)
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
        
        var user = await applicationDbContext.Users.FindAsync([request.UserId], cancellationToken);

        if (user is null)
        {
            return new Error("UserNotFound", $"User with given id {request.UserId} does not exist");
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