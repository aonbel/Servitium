using Application.Features.Appointments.Response;
using Domain.Abstractions.Result;
using Domain.Entities.Core;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAllPossibleAppointmentTimesQuery(
    int ServiceId,
    Coordinates FromCoordinates,
    double Distance,
    DateTime FromDate,
    TimeSpan TimeSpan) : IRequest<Result<GetAllPossibleAppointmentTimesQueryResponse>>;