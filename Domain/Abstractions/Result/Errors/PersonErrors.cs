namespace Domain.Abstractions.Result.Errors;

public static class PersonErrors
{
    public static Error NotFoundById(int id) =>
        new Error("PersonNotFoundById", $"Person with given id {id} does not exist");
    
    public static Error NotFoundByUserId(string userId) =>
        new Error("PersonNotFoundByUserId", $"Person with given user id {userId} does not exist");
}