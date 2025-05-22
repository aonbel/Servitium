using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Queries;

public record GetSpecialistByUserIdQuery(string UserId) : IRequest<Result<Specialist>>;