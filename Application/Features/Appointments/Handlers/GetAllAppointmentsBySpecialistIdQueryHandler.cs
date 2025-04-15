using Application.Features.Appointments.Queries;
using Application.Interfaces;
using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Appointments.Handlers;

public sealed class GetAllAppointmentsBySpecialistIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllAppointmentsBySpecialistIdQuery, Result<ICollection<Appointment>>>
{
    public async Task<Result<ICollection<Appointment>>> Handle(GetAllAppointmentsBySpecialistIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await applicationDbContext.Appointments
            .Where(a => a.Client.Id == request.SpecialistId)
            .ToListAsync(cancellationToken);

        return appointments;
    }
}