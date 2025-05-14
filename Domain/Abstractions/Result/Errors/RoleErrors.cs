namespace Domain.Abstractions.Result.Errors;

public static class RoleErrors
{
    public static Error NotFoundByName(string name) =>
        new Error("RoleNotFoundByName", $"Role with given name {name} does not exist");
}