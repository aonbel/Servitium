namespace Domain.Abstractions.Result.Errors;

public static class SpecialistErrors
{
    public static Error NotFoundById(int id) =>
        new Error("SpecialistNotFoundById", $"Specialist with given id {id} does not exist");

    public static Error SpecialistDoesNotHaveService(int specialistId, int serviceId) =>
        new Error("SpecialistDoesNotHaveService",
            $"Service with given id {serviceId} does not exist among services of specialist with given id {specialistId}");
}