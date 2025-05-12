using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Users.Queries;

public sealed record GetAllUsersQuery() : IRequest<Result<ICollection<User>>>;