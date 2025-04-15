using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAppointmentByIdQuery(int AppointmentId) : IRequest<Result<Appointment>>;