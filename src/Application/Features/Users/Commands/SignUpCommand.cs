using Application.Features.Users.Responces;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Users.Commands;

public record SignUpCommand(string Username, string Password, ICollection<string> Roles) : IRequest<Result<SignUpCommandResponce>>;