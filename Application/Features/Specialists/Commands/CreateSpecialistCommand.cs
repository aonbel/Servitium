using Domain.Abstractions;
using Domain.Models.Entities.Core;
using MediatR;

namespace Application.Features.Specialists.Commands;

public sealed record CreateSpecialistCommand(
    int UserId,
    int ServiceProviderId,
    string FirstName,
    string MiddleName,
    string LastName,
    string Email,
    string Phone,
    decimal PricePerHour,
    TimeOnlySegment WorkTime,
    DayOfWeek[] WorkDays,
    string[] Contacts,
    string Location
) : IRequest<Result<int>>;