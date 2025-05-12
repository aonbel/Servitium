namespace Domain.Abstractions.Result;

public record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullError = new("Error.NullValue", string.Empty);
}