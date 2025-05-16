using Application.Features.Users.Commands;
using Application.Features.Users.Responces;
using Domain.Abstractions.RefreshToken;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Domain.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Options.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Handlers;

public sealed class SignInByTokenCommandHandler(
    IApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager,
    ITokenProvider tokenProvider,
    IOptions<AuthenticationOptions> authenticationOptions)
    : IRequestHandler<SignInByTokenCommand, Result<SignInByTokenCommandResponce>>
{
    public async Task<Result<SignInByTokenCommandResponce>> Handle(SignInByTokenCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken =
            await applicationDbContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == request.RefreshToken,
                cancellationToken);

        if (refreshToken is null)
        {
            return RefreshTokenErrors.NotFoundByToken();
        }
        
        var user = await userManager.Users.SingleOrDefaultAsync(
            u => u.Id == refreshToken.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFoundById(refreshToken.UserId);
        }

        var accessToken = await tokenProvider.GenerateAccessToken(user);
        
        applicationDbContext.RefreshTokens.Remove(refreshToken);

        var newRefreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresOn = DateTime.UtcNow.AddDays(authenticationOptions.Value.RefreshTokenExpirationInDays)
        };
        
        await applicationDbContext.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return new SignInByTokenCommandResponce(accessToken, newRefreshToken.Token);
    }
}