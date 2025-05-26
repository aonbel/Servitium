using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Queries;

public sealed record GetAllSpecialistsByServiceProviderIdAndServiceIdAndDateTimeQuery(int ServiceProviderId, int ServiceId, DateTime DateTime) : IRequest<Result<ICollection<Specialist>>>;