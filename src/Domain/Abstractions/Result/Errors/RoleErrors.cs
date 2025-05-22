namespace Domain.Abstractions.Result.Errors;

public static class RoleErrors
{
    public static Error NotFoundByName(string name) =>
        new ("RoleNotFoundByName", $"Role with given name {name} does not exist");
}