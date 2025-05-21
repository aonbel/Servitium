using Microsoft.AspNetCore.Identity;

namespace Application.Features.Users.Responces;

public record SignUpCommandResponce(string AccessToken, string RefreshToken, IdentityUser User);