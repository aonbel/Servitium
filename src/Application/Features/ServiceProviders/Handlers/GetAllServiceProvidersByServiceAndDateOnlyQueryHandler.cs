using Application.Features.ServiceProviders.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Core;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using MongoDB.Driver.Linq;

namespace Application.Features.ServiceProviders.Handlers;

public class GetAllServiceProvidersByServiceAndDateOnlyQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllServiceProvidersByServiceAndDateTimeQuery, Result<ICollection<ServiceProvider>>>
{
    public async Task<Result<ICollection<ServiceProvider>>> Handle(
        GetAllServiceProvidersByServiceAndDateTimeQuery request, CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var dateOnly = DateOnly.FromDateTime(request.DateTime);
        var timeSpan = TimeOnly.FromDateTime(request.DateTime).ToTimeSpan();

        var specialistsThatCanProvideServiceAtDate = applicationDbContext.Specialists
            .Where(s => s.ServiceIds.Contains(request.ServiceId) && s.WorkDays.Contains(dateOnly.DayOfWeek))
            .ToList();

        List<ServiceProvider> serviceProviders = [];
        
        HashSet<int> addedServiceProviderIds = [];

        foreach (var specialist in specialistsThatCanProvideServiceAtDate)
        {
            if (addedServiceProviderIds.Contains(specialist.ServiceProviderId))
            {
                continue;
            }
            
            var appointments = applicationDbContext.Appointments
                .Where(a => a.SpecialistId == specialist.Id && a.Date == dateOnly && a.TimeSegment.End > timeSpan)
                .OrderBy(a => a.TimeSegment.Begin)
                .ToList();

            // Dummies

            appointments.Insert(0, new Appointment
            {
                ClientId = 0,
                ServiceId = 0,
                SpecialistId = 0,
                Date = new DateOnly(),
                TimeSegment = new TimeOnlySegment(TimeSpan.Zero, timeSpan),
            });

            appointments.Add(new Appointment
            {
                ClientId = 0,
                ServiceId = 0,
                SpecialistId = 0,
                Date = new DateOnly(),
                TimeSegment = new TimeOnlySegment(specialist.WorkTime.End, TimeSpan.FromDays(1)),
            });

            if (!appointments.Where((t, index) =>
                    index + 1 != appointments.Count &&
                    appointments[index + 1].TimeSegment.Begin > t.TimeSegment.End &&
                    appointments[index + 1].TimeSegment.Begin - t.TimeSegment.End >= service.Duration).Any()) continue;

            var serviceProvider = await applicationDbContext.ServiceProviders.FindAsync(
                [specialist.ServiceProviderId],
                cancellationToken);

            if (serviceProvider is null)
            {
                return ServiceErrors.NotFoundById(specialist.ServiceProviderId);
            }

            serviceProviders.Add(serviceProvider);
            addedServiceProviderIds.Add(serviceProvider.Id ?? 0);
        }

        return serviceProviders;
    }
}