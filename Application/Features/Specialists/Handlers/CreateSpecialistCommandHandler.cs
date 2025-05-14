using Application.Features.Specialists.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public sealed class CreateSpecialistCommandHandler(
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateSpecialistCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateSpecialistCommand request, CancellationToken cancellationToken)
    {
        var personExists = await applicationDbContext.Persons.AnyAsync(
            p => p.Id == request.PersonId,
            cancellationToken);

        if (!personExists)
        {
            return PersonErrors.NotFoundById(request.PersonId);
        }

        var serviceProviderExists = await applicationDbContext.ServiceProviders.AnyAsync(
            sp => sp.Id == request.ServiceProviderId,
            cancellationToken);

        if (!serviceProviderExists)
        {
            return ServiceProviderErrors.NotFoundById(request.ServiceProviderId);
        }

        var specialist = new Specialist
        {
            PersonId = request.PersonId,
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