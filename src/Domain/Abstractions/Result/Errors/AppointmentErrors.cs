namespace Domain.Abstractions.Result.Errors;

public static class AppointmentErrors
{
    public static Error NotFoundById(int id) =>
        new Error("AppointmentNotFoundById", $"Appointment with given id {id} does not exist");
    
    public static Error AppointmentAlreadyExists() =>
        new ("AppointmentAlreadyExists", "Appointment at given time already exists");

    public static Error AppointmentIsNotAtTimeOfWork() =>
        new ("AppointmentIsNotAtTimeOfWork", "Given time segment is not at work time of specialist");
}