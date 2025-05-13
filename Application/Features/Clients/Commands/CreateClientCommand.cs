using Domain.Abstractions;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Clients.Commands;

public sealed record CreateClientCommand(
    string UserId,
    string FirstName,
    string MiddleName,
    string LastName,
    string Email, 
    string Phone, 
    DateOnly Birthday,
    string Gender) : IRequest<Result<int>>;