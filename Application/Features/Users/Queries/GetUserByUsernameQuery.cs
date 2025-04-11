using Domain.Abstractions;
using Domain.Models.Entities.People;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserByUsernameQuery(string Username) : IRequest<Result<User>>;