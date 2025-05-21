using Domain.Abstractions;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Users.Commands;

public sealed record UpdateUserPasswordCommand(string Id, string Password) : IRequest<Result>;