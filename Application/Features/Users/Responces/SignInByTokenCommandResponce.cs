namespace Application.Features.Users.Responces;

public record SignInByTokenCommandResponce(string AccessToken, string RefreshToken);