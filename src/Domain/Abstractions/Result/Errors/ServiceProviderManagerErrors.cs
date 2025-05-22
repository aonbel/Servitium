namespace Domain.Abstractions.Result.Errors;

public static class ServiceProviderManagerErrors
{
    public static Error NotFoundById(int id) =>
        new ("ServiceProviderManagerNotFoundById", $"Service provider with given id {id} does not exist");

    public static Error NotFoundByPersonId(int personId) =>
        new ("ServiceProviderManagerNotFoundByPersonId",$"Service provider with given person id {personId} does not exist");
}