using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Appointments.Commands;

public record DeleteAppointmentByIdCommand(int AppointmentId) : IRequest<Result>;