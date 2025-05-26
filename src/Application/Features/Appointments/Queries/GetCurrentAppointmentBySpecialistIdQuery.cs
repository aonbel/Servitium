using Application.Features.Appointments.Response;
using Domain.Abstractions.Result;
using Domain.Entities.Services;
using MediatR;

namespace Application.Features.Appointments.Queries;

public record GetAppointmentBySpecialistIdAndDateTimeQuery(int SpecialistId, DateTime DateTime) : IRequest<Result<GetAppointmentBySpecialistIdAndDateTimeQueryResponse>>;