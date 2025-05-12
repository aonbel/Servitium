namespace Application.Features.Users.Responces;

public record SignUpCommandResponce(string AccessToken, string RefreshToken, int UserId);