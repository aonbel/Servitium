namespace Domain.Abstractions.Result.Errors;

public static class RefreshTokenErrors
{
    public static Error NotFoundByToken() =>
        new ("TokenNotFound", "Given refresh token does not exist");
}