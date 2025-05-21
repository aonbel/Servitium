using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAllAppointmentsBySpecialistIdQuery(int SpecialistId) : IRequest<Result<ICollection<Appointment>>>;