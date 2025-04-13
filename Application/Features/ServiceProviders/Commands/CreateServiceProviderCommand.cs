using Domain.Abstractions;
using Domain.Models.Entities.Core;
using Domain.Models.Entities.People;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.ServiceProviders.Commands;

public sealed record CreateServiceProviderCommand(
    string Name,
    string ShortName,
    string Address,
    Coordinates Coordinates,
    TimeOnlySegment WorkTime,
    DayOfWeek[] WorkDays,
    string[] Contacts
) : IRequest<Result<int>>;