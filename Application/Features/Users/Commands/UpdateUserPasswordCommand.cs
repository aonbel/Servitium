using Domain.Abstractions;
using MediatR;

namespace Application.Features.Users.Commands;

public record UpdateUserPasswordCommand(int Id, string Password) : IRequest<Result>;