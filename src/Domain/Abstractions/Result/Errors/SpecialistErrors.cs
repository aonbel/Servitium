namespace Domain.Abstractions.Result.Errors;

public static class SpecialistErrors
{
    public static Error NotFoundById(int id) =>
        new ("SpecialistNotFoundById", $"Specialist with given id {id} does not exist");
    
    public static Error NotFoundByPersonId(int personId) =>
        new ("SpecialistNotFoundByPersonId", $"Specialist with given person id {personId} does not exist");

    public static Error SpecialistDoesNotHaveService(int specialistId, int serviceId) =>
        new ("SpecialistDoesNotHaveService",
            $"Service with given id {serviceId} does not exist among services of specialist with given id {specialistId}");

    public static Error SpecialistDoesNotWorkAt(int specialistId, DateOnly date) =>
        new ("SpecialistDoesNotWorkAt", $"Specialist with given id {specialistId} does not work at date {date}");
}