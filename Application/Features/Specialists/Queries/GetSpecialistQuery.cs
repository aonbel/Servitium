using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Specialists.Queries;

public sealed record GetSpecialistQuery(int Id) : IRequest<Result<Specialist>>;