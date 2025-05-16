using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Core;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Commands;

public sealed record CreateSpecialistCommand(
    int PersonId,
    int ServiceProviderId,
    decimal PricePerHour,
    TimeOnlySegment WorkTime,
    ICollection<DayOfWeek> WorkDays,
    ICollection<string> Contacts,
    string Location) : IRequest<Result<Specialist>>;