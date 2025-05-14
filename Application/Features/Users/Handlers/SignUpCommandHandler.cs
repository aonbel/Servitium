using Application.Features.Users.Commands;
using Application.Features.Users.Responces;
using Domain.Abstractions.RefreshToken;
using Domain.Abstractions.Result;
using Domain.Abstractions.Result.Errors;
using Infrastructure.Interfaces;
using Infrastructure.Options.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.Features.Users.Handlers;

public sealed class SignUpCommandHandler(
    UserManager<IdentityUser> userManager, 
    IApplicationDbContext applicationDbContext,
    ITokenProvider tokenProvider,
    RoleManager<IdentityRole> roleManager,
    IOptions<AuthenticationOptions> authenticationOptions)
    : IRequestHandler<SignUpCommand, Result<SignUpCommandResponce>>
{
    public async Task<Result<SignUpCommandResponce>> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.Users.AnyAsync(
                u => u.UserName == request.Username,
                cancellationToken: cancellationToken))
        {
            return UserErrors.UsernameAlreadyExists(request.Username);
        }

        if (request.Roles.Count == 0)
        {
            return UserErrors.RolesNumberShouldBeAtLeastOne();
        }
        
        var user = new IdentityUser
        {
            UserName = request.Username,
            PasswordHash = request.Password
        };
        
        await userManager.CreateAsync(user);

        foreach (var role in request.Roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                return RoleErrors.RoleNotFoundByName(role);
            }
            
            await userManager.AddToRoleAsync(user, role);
        }
        
        var accessToken = await tokenProvider.GenerateAccessToken(user);
        var refreshToken = new RefreshToken
        {
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiresOn = DateTime.UtcNow.AddDays(authenticationOptions.Value.RefreshTokenExpirationInDays),
            UserId = user.Id!,
        };
        
        await applicationDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return new SignUpCommandResponce(accessToken, refreshToken.Token, user.Id);
    }
}