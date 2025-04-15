using Application.Features.Appointments.Commands;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
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
            return new Error("SpecialistNotFound", $"Specialist with given id {request.SpecialistId} does not exist");
        }

        var isTimeFreeFromAppointments = await applicationDbContext.Appointments
            .Where(a => a.Specialist.Id == request.SpecialistId)
            .AllAsync(a => a.Date != request.Date || !request.TimeSegment.IsIntersecting(a.TimeSegment),
                cancellationToken);

        if (!isTimeFreeFromAppointments)
        {
            return new Error("TimeSegmentIsInvalid", "There already exists an appointment at given time");
        }

        var isInWorkTime = specialist.WorkDays.Contains(request.Date.DayOfWeek) &&
                           specialist.WorkTime.Contains(request.TimeSegment);

        if (!isInWorkTime)
        {
            return new Error("TimeSegmentIsInvalid", "Given time segment is not at work time of specialist");
        }
        
        var service = specialist.Services.FirstOrDefault(s => s.Id == request.ServiceId);

        if (service is null)
        {
            return new Error("ServiceNotFound",
                $"Service with given id {request.ServiceId} does not exist among specialist services");
        }

        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return new Error("ClientNotFound", $"Client with given id {request.ClientId} does not exist");
        }
        
        var appointment = new Appointment
        {
            Client = client,
            Specialist = specialist,
            Service = service,
            Date = request.Date,
            TimeSegment = request.TimeSegment
        };

        await applicationDbContext.Appointments.AddAsync(appointment, cancellationToken);

        return await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}