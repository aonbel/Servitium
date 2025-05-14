namespace Domain.Abstractions.Result.Errors;

public static class PersonErrors
{
    public static Error NotFoundById(int id) =>
        new Error("PersonNotFoundById", $"Person with given id {id} does not exist");
}