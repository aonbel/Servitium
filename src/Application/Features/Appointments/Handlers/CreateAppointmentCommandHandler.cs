using Application.Features.Appointments.Commands;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Appointments.Handlers;

public sealed class CreateAppointmentCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CreateAppointmentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var specialist = await applicationDbContext.Specialists.FindAsync([request.SpecialistId], cancellationToken);

        if (specialist is null)
        {
            return SpecialistErrors.NotFoundById(request.SpecialistId);
        }
        
        // TODO Optimize
        
        var isTimeFreeFromAppointments = applicationDbContext.Appointments
            .Where(a => a.SpecialistId == request.SpecialistId)
            .AsEnumerable()
            .All(a => a.Date != request.Date || !request.TimeSegment.IsIntersecting(a.TimeSegment));

        if (!isTimeFreeFromAppointments)
        {
            return AppointmentErrors.AppointmentAlreadyExists();
        }

        var isInWorkTime = specialist.WorkDays.Contains(request.Date.DayOfWeek) &&
                           specialist.WorkTime.Contains(request.TimeSegment);

        if (!isInWorkTime)
        {
            return AppointmentErrors.AppointmentIsNotAtTimeOfWork();
        }

        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var specialistCanProvideService = specialist.ServiceIds.Contains(request.ServiceId);

        if (!specialistCanProvideService)
        {
            return SpecialistErrors.SpecialistDoesNotHaveService(specialist.Id ?? 0, request.ServiceId);
        }

        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return new Error("ClientNotFound", $"Client with given id {request.ClientId} does not exist");
        }

        var appointment = new Appointment
        {
            ClientId = request.ClientId,
            SpecialistId = request.SpecialistId,
            ServiceId = request.ServiceId,
            Date = request.Date,
            TimeSegment = request.TimeSegment
        };

        await applicationDbContext.Appointments.AddAsync(appointment, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}