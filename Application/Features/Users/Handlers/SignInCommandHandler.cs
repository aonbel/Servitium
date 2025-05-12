using Application.Features.Users.Commands;
using Application.Features.Users.Responces;
using Domain.Abstractions.RefreshToken;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using Infrastructure.Options.Authentication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Handlers;

public sealed class SignInCommandHandler(
    IApplicationDbContext applicationDbContext,
    ITokenProvider tokenProvider,
    IOptions<AuthenticationOptions> authenticationOptions)
    : IRequestHandler<SignInCommand, Result<SignInCommandResponce>>
{
    public async Task<Result<SignInCommandResponce>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var user = await applicationDbContext.Users.FirstOrDefaultAsync(
            user => user.Username == request.Username,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFoundByUsername(request.Username);
        }

        if (user.Password != request.Password)
        {
            return UserErrors.WrongPassword();
        }

        var oldRefreshToken =
            await applicationDbContext.RefreshTokens.FirstOrDefaultAsync(
                token => token.UserId == user.Id,
                cancellationToken);

        if (oldRefreshToken is not null)
        {
            applicationDbContext.RefreshTokens.Remove(oldRefreshToken);
        }

        var accessToken = tokenProvider.GenerateAccessToken(user);
        var refreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOn = DateTime.UtcNow.AddDays(authenticationOptions.Value.RefreshTokenExpirationInDays),
            UserId = user.Id!.Value,
        };

        await applicationDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return new SignInCommandResponce(accessToken, refreshToken.Token);
    }
}