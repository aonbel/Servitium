namespace Domain.Abstractions.Result.Errors;

public static class ClientErrors
{
    public static Error NotFoundByPersonId(int personId) =>
        new ("NotFoundByPersonId", $"The client with given person id {personId} does not exist");

    public static Error NotFoundById(int id) =>
        new ("NotFoundById", $"The client with given id {id} does not exist");
}