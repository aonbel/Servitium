using Domain.Entities.Services;

namespace Application.Features.Appointments.Response;

public record GetAppointmentBySpecialistIdAndDateTimeQueryResponse(Appointment? Appointment);