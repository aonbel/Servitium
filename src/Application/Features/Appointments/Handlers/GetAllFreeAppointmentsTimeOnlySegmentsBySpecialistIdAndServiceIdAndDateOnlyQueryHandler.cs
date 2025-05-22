using Application.Features.Appointments.Queries;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Core;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Appointments.Handlers;

public class GetAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQueryHandler(
    IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQuery,
        Result<ICollection<TimeOnlySegment>>>
{
    public async Task<Result<ICollection<TimeOnlySegment>>> Handle(
        GetAllFreeAppointmentsTimeOnlySegmentsBySpecialistIdAndServiceIdAndDateOnlyQuery request,
        CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var specialist = await applicationDbContext.Specialists.FindAsync([request.SpecialistId], cancellationToken);

        if (specialist is null)
        {
            return SpecialistErrors.NotFoundById(request.SpecialistId);
        }

        if (!specialist.WorkDays.Contains(request.Date.DayOfWeek))
        {
            return SpecialistErrors.SpecialistDoesNotWorkAt(request.SpecialistId, request.Date);
        }

        var appointments = applicationDbContext.Appointments
            .Where(a => a.SpecialistId == request.SpecialistId && a.Date == request.Date)
            .OrderBy(a => a.TimeSegment.Begin);

        List<TimeOnlySegment> timeOnlySegments = [];

        var currentBeginOfTimeSegment = specialist.WorkTime.Begin;

        foreach (var appointment in appointments)
        {
            var freeSegments = GetAllFreeTimeSegmentsBetween(
                currentBeginOfTimeSegment, 
                appointment.TimeSegment.Begin,
                service.Duration);
            
            timeOnlySegments.AddRange(freeSegments);

            currentBeginOfTimeSegment = appointment.TimeSegment.End;
        }

        {
            var freeSegments = GetAllFreeTimeSegmentsBetween(
                currentBeginOfTimeSegment, 
                specialist.WorkTime.End,
                service.Duration);
        
            timeOnlySegments.AddRange(freeSegments);
        }
        
        return timeOnlySegments;
    }

    private ICollection<TimeOnlySegment> GetAllFreeTimeSegmentsBetween(
        TimeOnly start,
        TimeOnly end,
        TimeSpan serviceDuration)
    {
        List<TimeOnlySegment> result = [];

        var timeSegmentEndInSpan = end.ToTimeSpan();

        for (var newFreeTimeOnlySegmentBegin = start.ToTimeSpan();
             newFreeTimeOnlySegmentBegin + serviceDuration < timeSegmentEndInSpan;
             newFreeTimeOnlySegmentBegin += serviceDuration)
        {
            result.Add(new TimeOnlySegment(
                TimeOnly.FromTimeSpan(newFreeTimeOnlySegmentBegin),
                TimeOnly.FromTimeSpan(newFreeTimeOnlySegmentBegin + serviceDuration)));
        }

        return result;
    }
}