using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAllAppointmentsBySpecialistIdQuery(int SpecialistId) : IRequest<Result<ICollection<Appointment>>>;