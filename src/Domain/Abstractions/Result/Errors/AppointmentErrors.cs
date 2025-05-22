namespace Domain.Abstractions.Result.Errors;

public static class AppointmentErrors
{
    public static Error AppointmentAlreadyExists() =>
        new ("AppointmentAlreadyExists", "Appointment at given time already exists");

    public static Error AppointmentIsNotAtTimeOfWork() =>
        new ("AppointmentIsNotAtTimeOfWork", "Given time segment is not at work time of specialist");
}