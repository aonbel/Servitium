using Application.Features.Appointments.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
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
            .Where(a => a.SpecialistId == request.SpecialistId)
            .ToListAsync(cancellationToken);

        return appointments;
    }
}