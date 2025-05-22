using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Queries;

public sealed record GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateOnlyQuery(int ServiceProviderId, int ServiceId, DateOnly Date) : IRequest<Result<ICollection<Specialist>>>;