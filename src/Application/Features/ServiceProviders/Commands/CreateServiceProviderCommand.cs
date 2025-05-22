using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.Core;
using MediatR;

namespace Application.Features.ServiceProviders.Commands;

public sealed record CreateServiceProviderCommand(
    string Name,
    string ShortName,
    string Address,
    TimeOnlySegment WorkTime,
    ICollection<DayOfWeek> WorkDays,
    ICollection<string> Contacts
) : IRequest<Result<int>>;