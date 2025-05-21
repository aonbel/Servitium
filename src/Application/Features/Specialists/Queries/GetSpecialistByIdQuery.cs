using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Queries;

public sealed record GetSpecialistByIdQuery(int Id) : IRequest<Result<Specialist>>;