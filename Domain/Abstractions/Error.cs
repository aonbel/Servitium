namespace Domain.Abstractions;

public record Error(string Code, string Message)
{
    public static Error None = new(string.Empty, string.Empty);
    public static Error NullError = new("Error.NullValue", string.Empty);
}