using Domain.Abstractions;
using MediatR;

namespace Application.Features.Clients.Commands;

public sealed record CreateClientCommand(
    int UserId,
    string FirstName,
    string MiddleName,
    string LastName,
    string Email, 
    string Phone, 
    DateOnly Birthday,
    string Gender) : IRequest<Result<int>>;