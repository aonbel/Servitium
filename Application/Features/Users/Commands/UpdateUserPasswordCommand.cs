using Domain.Abstractions;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Users.Commands;

public sealed record UpdateUserPasswordCommand(int Id, string Password) : IRequest<Result>;