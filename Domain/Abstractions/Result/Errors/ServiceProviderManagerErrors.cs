namespace Domain.Abstractions.Result.Errors;

public static class ServiceProviderManagerErrors
{
    public static Error NotFoundById(int id) =>
        new Error("ServiceProviderManagerNotFoundById", $"Service provider with given id {id} does not exist");
}