using Application.Features.Specialists.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Core;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Specialists.Handlers;

public sealed class GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQueryHandler(
    IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateTimeQuery, Result<ICollection<Specialist>>>
{
    public async Task<Result<ICollection<Specialist>>> Handle(
        GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateTimeQuery request,
        CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var dateOnly = DateOnly.FromDateTime(request.DateTime);
        var timeOnly = TimeOnly.FromDateTime(request.DateTime);

        var specialistsThatCanProvideServiceAtDate = applicationDbContext.Specialists
            .Where(s => s.ServiceIds.Contains(request.ServiceId) && s.WorkDays.Contains(dateOnly.DayOfWeek))
            .ToList();

        List<Specialist> specialists = [];

        foreach (var specialist in specialistsThatCanProvideServiceAtDate)
        {
            var appointments = applicationDbContext.Appointments
                .Where(a => a.SpecialistId == specialist.Id && a.Date == dateOnly && a.TimeSegment.End > timeOnly)
                .OrderBy(a => a.TimeSegment.Begin)
                .ToList();

            // Dummies

            appointments.Insert(0, new Appointment
            {
                ClientId = 0,
                ServiceId = 0,
                SpecialistId = 0,
                Date = new DateOnly(),
                TimeSegment = new TimeOnlySegment(new TimeOnly(), timeOnly),
            });

            appointments.Add(new Appointment
            {
                ClientId = 0,
                ServiceId = 0,
                SpecialistId = 0,
                Date = new DateOnly(),
                TimeSegment = new TimeOnlySegment(specialist.WorkTime.End, new TimeOnly(23, 59, 59)),
            });

            if (!appointments.Where((t, index) =>
                    index + 1 != appointments.Count &&
                    appointments[index + 1].TimeSegment.Begin > t.TimeSegment.End &&
                    appointments[index + 1].TimeSegment.Begin - t.TimeSegment.End >= service.Duration).Any()) continue;

            specialists.Add(specialist);
        }

        return specialists;
    }
}