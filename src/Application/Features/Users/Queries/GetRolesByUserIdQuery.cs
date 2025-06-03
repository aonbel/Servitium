using Domain.Abstractions.Result;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetRolesByUserIdQuery(string UserId) : IRequest<Result<ICollection<string>>>;