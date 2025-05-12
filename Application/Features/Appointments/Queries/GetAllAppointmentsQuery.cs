using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAllAppointmentsQuery() : IRequest<Result<ICollection<Appointment>>>;