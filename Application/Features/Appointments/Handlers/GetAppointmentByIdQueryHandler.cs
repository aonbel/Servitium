using Application.Features.Appointments.Queries;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Appointments.Handlers;

public sealed class GetAppointmentByIdQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAppointmentByIdQuery, Result<Appointment>>
{
    public async Task<Result<Appointment>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await applicationDbContext.Appointments.FindAsync([request.AppointmentId], cancellationToken);

        if (appointment is null)
        {
            return new Error("AppointmentNotFound",
                $"Appointment with given id {request.AppointmentId} does not exist");
        }
        
        return appointment;
    }
}