using Application.Features.ServiceProviders.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Handlers;

public sealed class CreateServiceProviderCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateServiceProviderCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateServiceProviderCommand request, CancellationToken cancellationToken)
    {
        var serviceProvider = new ServiceProvider
        {
            Name = request.Name,
            ShortName = request.ShortName,
            Address = request.Address,
            Coordinates = request.Coordinates,
            Contacts = request.Contacts,
            WorkTime = request.WorkTime,
            WorkDays = request.WorkDays,
            Services = [],
            Specialists = []
        };

        await applicationDbContext.ServiceProviders.AddAsync(serviceProvider, cancellationToken);

        var id = await applicationDbContext.SaveChangesAsync(cancellationToken);

        return id;
    }
}