using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Queries;

public sealed record GetSpecialistsByServiceProviderIdQuery(int ServiceProviderId) : IRequest<Result<ICollection<Specialist>>>;