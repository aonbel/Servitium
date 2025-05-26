using Application.Features.Appointments.Queries;
using Application.Features.Appointments.Response;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Appointments.Handlers;

public class GetAppointmentBySpecialistIdAndDateTimeQueryHandler(IApplicationDbContext applicationDbContext) :
    IRequestHandler<GetAppointmentBySpecialistIdAndDateTimeQuery, Result<GetAppointmentBySpecialistIdAndDateTimeQueryResponse>>
{
    public async Task<Result<GetAppointmentBySpecialistIdAndDateTimeQueryResponse>> Handle(GetAppointmentBySpecialistIdAndDateTimeQuery request,
        CancellationToken cancellationToken)
    {
        var specialist = await applicationDbContext.Specialists.FindAsync([request.SpecialistId], cancellationToken);

        if (specialist is null)
        {
            return SpecialistErrors.NotFoundById(request.SpecialistId);
        }

        var appointments = applicationDbContext.Appointments
            .Where(a => a.SpecialistId == specialist.Id);

        var dateOnly = DateOnly.FromDateTime(request.DateTime);
        var timeOnly = TimeOnly.FromDateTime(request.DateTime);

        foreach (var appointment in appointments)
        {
            if (appointment.Date == dateOnly && appointment.TimeSegment.Contains(timeOnly))
            {
                return new GetAppointmentBySpecialistIdAndDateTimeQueryResponse(appointment);
            }
        }

        return new GetAppointmentBySpecialistIdAndDateTimeQueryResponse(null);
    }
}