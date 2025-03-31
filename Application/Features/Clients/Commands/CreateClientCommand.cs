using MediatR;

namespace Application.Features.Clients.Commands;

public record CreateClientCommand(
    string FirstName,
    string MiddleName,
    string LastName,
    string Email, 
    string Phone, 
    DateOnly Birthday,
    string Gender) : IRequest<int>;