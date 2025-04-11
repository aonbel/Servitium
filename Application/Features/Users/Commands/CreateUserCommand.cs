using Domain.Abstractions;
using MediatR;

namespace Application.Features.Users.Commands;

public record CreateUserCommand(
    string Password, 
    string Username, 
    string Role) : IRequest<Result<int>>;