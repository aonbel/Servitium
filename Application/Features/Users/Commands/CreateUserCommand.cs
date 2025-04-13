using Domain.Abstractions;
using MediatR;

namespace Application.Features.Users.Commands;

public sealed record CreateUserCommand(
    string Password, 
    string Username, 
    string Role) : IRequest<Result<int>>;