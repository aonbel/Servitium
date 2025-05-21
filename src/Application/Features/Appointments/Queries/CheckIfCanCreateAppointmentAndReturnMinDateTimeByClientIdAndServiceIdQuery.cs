using Application.Features.Appointments.Response;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQuery(int ClientId, int ServiceId) : IRequest<Result<CheckIfCanCreateAppointmentAndReturnMinDateTimeByClientIdAndServiceIdQueryResponse>>;