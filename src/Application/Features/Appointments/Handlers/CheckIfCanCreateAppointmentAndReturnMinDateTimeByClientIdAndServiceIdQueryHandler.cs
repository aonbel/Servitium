using Application.Features.Appointments.Queries;
using Application.Features.Appointments.Response;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.Services;
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Appointments.Handlers;

public class CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryHandler(
    IApplicationDbContext applicationDbContext)
    : IRequestHandler<CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery, Result<CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse>>
{
    public async Task<Result<CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse>> Handle(
        CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery request,
        CancellationToken cancellationToken)
    {
        var client = await applicationDbContext.Clients.FindAsync([request.ClientId], cancellationToken);

        if (client is null)
        {
            return ClientErrors.NotFoundById(request.ClientId);
        }

        var service = await applicationDbContext.Services.FindAsync([request.ServiceId], cancellationToken);

        if (service is null)
        {
            return ServiceErrors.NotFoundById(request.ServiceId);
        }

        var healthCertificates = applicationDbContext.HealthCertificates.Where(c => c.ClientId == request.ClientId);

        var appointments = applicationDbContext.Appointments.Where(c => c.ClientId == request.ClientId);

        List<(Service Service, Appointment Appointment)> servicesFromAppointments = [];

        foreach (var appointment in appointments)
        {
            var serviceFromAppointment =
                await applicationDbContext.Services.FindAsync([appointment.ServiceId], cancellationToken);

            if (serviceFromAppointment is null)
            {
                return ServiceErrors.NotFoundById(appointment.ServiceId);
            }

            servicesFromAppointments.Add((serviceFromAppointment, appointment));
        }

        var minDateTime = DateTime.Now;

        foreach (var requiredTemplateId in service.RequiredHealthCertificateTemplateIds)
        {
            if (await healthCertificates.AnyAsync(c => c.TemplateId == requiredTemplateId, cancellationToken))
            {
                continue;
            }

            var suitableAppointments = servicesFromAppointments
                .Where(s => s.Service.ResultHealthCertificateTemplateIds.Contains(requiredTemplateId))
                .OrderBy(s => s.Appointment.Date)
                .ToList();

            if (suitableAppointments.Count != 0)
            {
                var minDateTimeForCurrentAppointment = suitableAppointments[0].Appointment.Date
                    .ToDateTime(suitableAppointments[0].Appointment.TimeSegment.End);

                if (minDateTimeForCurrentAppointment > minDateTime)
                {
                    minDateTime = minDateTimeForCurrentAppointment;
                }
                
                continue;
            }

            return new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse(false, null);
        }

        return new CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse(true, minDateTime);
    }
}