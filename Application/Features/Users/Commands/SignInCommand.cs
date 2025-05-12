using Application.Features.Users.Responces;
using Domain.Abstractions;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Users.Commands;

public record SignInCommand(string Username, string Password) : IRequest<Result<SignInCommandResponce>>;