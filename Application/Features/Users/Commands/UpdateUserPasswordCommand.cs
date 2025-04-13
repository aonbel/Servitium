using Domain.Abstractions;
using MediatR;

namespace Application.Features.Users.Commands;

public sealed record UpdateUserPasswordCommand(int Id, string Password) : IRequest<Result>;