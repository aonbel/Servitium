using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Core;
using MediatR;

namespace Application.Features.Specialists.Commands;

public sealed record CreateSpecialistCommand(
    int PersonId,
    int ServiceProviderId,
    decimal PricePerHour,
    TimeOnlySegment WorkTime,
    DayOfWeek[] WorkDays,
    string[] Contacts,
    string Location) : IRequest<Result<int>>;