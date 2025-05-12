namespace Application.Features.Users.Responces;

public sealed record SignInCommandResponce(string AccessToken, string RefreshToken);