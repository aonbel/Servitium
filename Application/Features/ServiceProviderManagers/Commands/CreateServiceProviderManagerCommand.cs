using Domain.Abstractions;
using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.ServiceProviderManagers.Commands;

public sealed record CreateServiceProviderManagerCommand(
    int UserId,
    int ServiceProviderId,
    string FirstName,
    string MiddleName,
    string LastName,
    string Email,
    string Phone) : IRequest<Result<int>>;