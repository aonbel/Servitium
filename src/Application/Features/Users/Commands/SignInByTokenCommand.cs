using Application.Features.Users.Responces;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Users.Commands;

public sealed record SignInByTokenCommand(string RefreshToken) : IRequest<Result<SignInByTokenCommandResponce>>;