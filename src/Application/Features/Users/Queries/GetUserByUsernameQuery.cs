using Domain.Abstractions;
using Domain.Abstractions.Result;
using Domain.Entities.People;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Queries;

public sealed record GetUserByUsernameQuery(string Username) : IRequest<Result<IdentityUser>>;