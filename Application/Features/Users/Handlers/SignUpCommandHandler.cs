using Application.Features.Users.Commands;
using Application.Features.Users.Responces;
using Domain.Abstractions.RefreshToken;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Entities.People;
using Domain.Interfaces;
using Infrastructure.Options.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Handlers;

public sealed class SignUpCommandHandler(
    IApplicationDbContext applicationDbContext, 
    ITokenProvider tokenProvider,
    IOptions<AuthenticationOptions> authenticationOptions)
    : IRequestHandler<SignUpCommand, Result<SignUpCommandResponce>>
{
    public async Task<Result<SignUpCommandResponce>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (await applicationDbContext.Users.AnyAsync(
                u => u.Username == request.Username,
                cancellationToken: cancellationToken))
        {
            return UserErrors.UsernameAlreadyExists(request.Username);
        }

        if (request.Roles.Count == 0)
        {
            return UserErrors.RolesNumberShouldBeAtLeastOne();
        }

        var user = new User
        {
            Username = request.Username,
            Password = request.Password,
            Roles = request.Roles
        };
        
        await applicationDbContext.Users.AddAsync(user, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        var accessToken = tokenProvider.GenerateAccessToken(user);
        var refreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOn = DateTime.UtcNow.AddDays(authenticationOptions.Value.RefreshTokenExpirationInDays),
            UserId = user.Id!.Value,
        };
        
        await applicationDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return new SignUpCommandResponce(accessToken, refreshToken.Token, user.Id!.Value);
    }
}