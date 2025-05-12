using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.Appointments.Queries;

public sealed record GetAllAppointmentsByClientIdQuery(int ClientId) : IRequest<Result<ICollection<Appointment>>>;