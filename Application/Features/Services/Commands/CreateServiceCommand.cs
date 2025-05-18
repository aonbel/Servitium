using Domain.Abstractions;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Services.Commands;

public sealed record CreateServiceCommand(
    string Name,
    string ShortName,
    string Description,
    ICollection<int> RequirementIds,
    ICollection<int> ResultIds,
    decimal PricePerHourForMaterials,
    decimal PricePerHourForEquipment,
    TimeSpan Duration) : IRequest<Result<int>>;