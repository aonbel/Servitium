using Application.Features.Specialists.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public sealed class CreateSpecialistCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateSpecialistCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSpecialistCommand request, CancellationToken cancellationToken)
    {
        var userExists = await applicationDbContext.Users.AnyAsync(
            u => u.Id == request.UserId, 
            cancellationToken);
        
        if (!userExists)
        {
            return new Error("UserNotFound", $"User with given id {request.UserId} does not exist");
        }
        
        var serviceProviderExists = await applicationDbContext.ServiceProviders.AnyAsync(
            sp => sp.Id == request.ServiceProviderId, 
            cancellationToken);
        
        if (!serviceProviderExists)
        {
            return new Error(
                "ServiceProviderNotFound", 
                $"Service provider with given id {request.ServiceProviderId} does not exist");
        }

        var specialist = new Specialist
        {
            UserId = request.UserId,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Contacts = request.Contacts,
            Location = request.Location,
            PricePerHour = request.PricePerHour,
            WorkTime = request.WorkTime,
            WorkDays = request.WorkDays,
            Services = []
        };
        
        await applicationDbContext.Specialists.AddAsync(specialist, cancellationToken);

        var id = await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        var serviceProvider = await applicationDbContext.ServiceProviders
            .Where(sp => sp.Id == request.ServiceProviderId)
            .FirstAsync(cancellationToken);
        
        serviceProvider.Specialists.Add(specialist);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return id;
    }
}