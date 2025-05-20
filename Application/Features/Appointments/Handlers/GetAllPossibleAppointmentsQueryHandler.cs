using Application.Features.Appointments.Queries;
using Application.Features.Appointments.Response;
using Application.Infrastructure;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Appointments.Handlers;

public class GetAllPossibleAppointmentsQueryHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<GetAllPossibleAppointmentTimesQuery, Result<GetAllPossibleAppointmentTimesQueryResponse>>
{
    /// <summary>
    /// Handles queries to retrieve all possible appointment times for a given service within a specified distance and date range.
    /// Filters service providers by proximity, checks specialist availability, and calculates open time slots based on existing appointments and work schedules.
    /// </summary>
    public async Task<Result<GetAllPossibleAppointmentTimesQueryResponse>> Handle(
        GetAllPossibleAppointmentTimesQuery request,
        CancellationToken cancellationToken)
    {
        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        List<(int ServiceProviderId, int SpecialistId, DateOnly Date, TimeOnly Time)> result = [];

        var serviceProviders = await applicationDbContext.ServiceProviders
            .Where(sp => DistanceCounter.GetDistance(request.FromCoordinates, sp.Coordinates) < request.Distance)
            .ToListAsync(cancellationToken);

        foreach (var serviceProvider in serviceProviders)
        {
            var specialists = await applicationDbContext.Specialists
                .Where(s => s.ServiceProviderId == serviceProvider.Id)
                .ToListAsync(cancellationToken);

            foreach (var specialist in specialists)
            {
                var canProvideService = specialist.ServiceIds.Contains(request.ServiceId);

                if (!canProvideService)
                {
                    continue;
                }

                var startingDate = new DateOnly(
                    request.FromDate.Year,
                    request.FromDate.Month,
                    request.FromDate.Day);

                for (var dayOffset = 0; dayOffset < request.TimeSpan.Days; dayOffset++)
                {
                    var day = startingDate.AddDays(dayOffset);

                    var startingTime = dayOffset == 0 ? TimeOnly.FromDateTime(request.FromDate) : new TimeOnly();

                    var endingTime = specialist.WorkTime.End;

                    if (startingTime < specialist.WorkTime.Begin)
                    {
                        startingTime = specialist.WorkTime.Begin;
                    }

                    var existingAppointments = await applicationDbContext.Appointments
                        .Where(a => a.SpecialistId == specialist.Id && a.Date == day)
                        .OrderBy(a => a.TimeSegment.Begin)
                        .ToListAsync(cancellationToken);

                    if (existingAppointments.Count == 0)
                    {
                        var allPossibleAtDay = GetAllAppointmentTimesFromTimeSegment(
                            startingTime,
                            endingTime,
                            service.Duration,
                            day,
                            specialist.Id ?? 0,
                            service.Id ?? 0);

                        result.AddRange(allPossibleAtDay);
                    }

                    var beginPossibilities = GetAllAppointmentTimesFromTimeSegment(
                        startingTime,
                        existingAppointments.First().TimeSegment.Begin,
                        service.Duration,
                        day,
                        specialist.Id ?? 0,
                        service.Id ?? 0);

                    var endPossibilities = GetAllAppointmentTimesFromTimeSegment(
                        existingAppointments.Last().TimeSegment.End,
                        endingTime,
                        service.Duration,
                        day,
                        specialist.Id ?? 0,
                        service.Id ?? 0);

                    result.AddRange(beginPossibilities);
                    result.AddRange(endPossibilities);

                    for (var index = 1; index < existingAppointments.Count; index++)
                    {
                        var currentPossibilities = GetAllAppointmentTimesFromTimeSegment(
                            existingAppointments[index - 1].TimeSegment.End,
                            existingAppointments[index].TimeSegment.Begin,
                            service.Duration,
                            day,
                            specialist.Id ?? 0,
                            service.Id ?? 0);

                        result.AddRange(currentPossibilities);
                    }
                }
            }
        }

        return new GetAllPossibleAppointmentTimesQueryResponse(result);
    }

    private List<(int ServiceProviderId, int SpecialistId, DateOnly Date, TimeOnly Time)>
        GetAllAppointmentTimesFromTimeSegment(
            TimeOnly begin,
            TimeOnly end,
            TimeSpan serviceDuration,
            DateOnly day,
            int specialistId,
            int serviceProviderId)
    {
        List<(int ServiceProviderId, int SpecialistId, DateOnly Date, TimeOnly Time)> result = [];

        var endInTimeSpan = end.ToTimeSpan();

        for (var currentTime = begin.ToTimeSpan();
             currentTime + serviceDuration <= endInTimeSpan;
             currentTime += serviceDuration)
        {
            result.Add((serviceProviderId, specialistId, day, TimeOnly.FromTimeSpan(currentTime)));
        }

        return result;
    }
}