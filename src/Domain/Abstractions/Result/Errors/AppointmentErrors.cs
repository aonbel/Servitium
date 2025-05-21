namespace Domain.Abstractions.Result.Errors;

public static class AppointmentErrors
{
    public static Error AppointmentAlreadyExists() =>
        new Error("AppointmentAlreadyExists", "Appointment at given time already exists");
    
    public static Error AppointmentIsNotAtTimeOfWork() =>
    new Error("AppointmentIsNotAtTimeOfWork", "Given time segment is not at work time of specialist");
}