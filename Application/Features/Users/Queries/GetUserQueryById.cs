using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;

namespace Application.Features.Users.Queries;

public sealed record GetUserQueryById(
    int UserId) : IRequest<Result<User>>;