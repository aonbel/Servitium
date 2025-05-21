using Application.Features.Appointments.Queries;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Appointments.Handlers;

public class GetAllAppointmentsQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetAllAppointmentsQuery, Result<ICollection<Appointment>>>
{
    public async Task<Result<ICollection<Appointment>>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await applicationDbContext.Appointments.ToListAsync(cancellationToken);
        
        return appointments;
    }
}