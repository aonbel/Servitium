namespace Domain.Abstractions.Result.Errors;

public static class ServiceErrors
{
    public static Error NotFoundById(int id) =>
        new Error("ServiceNotFoundById", $"Service with given id {id} does not exist");
}