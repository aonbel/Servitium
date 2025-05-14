namespace Domain.Abstractions.Result.Errors;

public static class SpecialistErrors
{
    public static Error NotFoundById(int id) =>
    new Error("SpecialistNotFoundById", $"Specialist with given id {id} does not exist");
}