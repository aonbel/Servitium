using Application.Features.Appointments.Commands;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Appointments.Handlers;

public class DeleteAppointmentByIdCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<DeleteAppointmentByIdCommand, Result>
{
    public async Task<Result> Handle(DeleteAppointmentByIdCommand request, CancellationToken cancellationToken)
    {
        var appointment = await applicationDbContext.Appointments.FindAsync([request.AppointmentId], cancellationToken);

        if (appointment is null)
        {
            return AppointmentErrors.NotFoundById(request.AppointmentId);
        }
        
        applicationDbContext.Appointments.Remove(appointment);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}