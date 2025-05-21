namespace Domain.Abstractions.Result.Errors;

public static class ServiceProviderErrors
{
    public static Error NotFoundById(int id) => 
        new Error("ServiceProviderNotFoundById", $"Service provider with given id {id} does not exist");
}