using Application.Features.Appointments.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Appointments.Handlers;

public sealed class GetAllAppointmentsByClientIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAllAppointmentsByClientIdQuery, Result<ICollection<Appointment>>>
{
    public async Task<Result<ICollection<Appointment>>> Handle(GetAllAppointmentsByClientIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await applicationDbContext.Appointments
            .Where(a => a.Client.Id == request.ClientId)
            .ToListAsync(cancellationToken);

        return appointments;
    }
}