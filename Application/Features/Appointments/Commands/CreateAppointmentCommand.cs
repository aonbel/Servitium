using Domain.Abstractions;
using Domain.Models.Entities.Core;
using MediatR;

namespace Application.Features.Appointments.Commands;

public record CreateAppointmentCommand(
    int ServiceId,
    int ClientId,
    int SpecialistId,
    DateOnly Date,
    TimeOnlySegment TimeSegment) : IRequest<Result<int>>;