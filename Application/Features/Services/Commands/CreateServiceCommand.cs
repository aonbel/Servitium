using Domain.Abstractions;
using Domain.Models.Entities.Services;
using MediatR;

namespace Application.Features.Services.Commands;

public sealed record CreateServiceCommand(
    string Name,
    string ShortName,
    string Description,
    ICollection<int> RequirementIds,
    ICollection<int> ResultIds,
    float PricePerHourForMaterials,
    float PricePerHourForEquipment,
    TimeSpan Duration) : IRequest<Result<int>>;