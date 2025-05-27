namespace Servitium.Infrastructure.PagesConstants;

public static class ErrorMessages
{
    // Shared

    public const string StringLengthRequirements = "The {0} must be at least {2} and at max {1} characters long.";
    public const string DurationRequirements = "The {0} must be at least {1} and at max {2}.";
    public const string DateOnlyRequirements = "The {0} must be not in the future or earlier than {1}.";
    
    // User
    
    public const string ConfirmPassword = "The password and confirmation password do not match.";
    
    // Service provider
    
    // Service
    
    public const string ServiceDuration = "The {0} must be at least {1} and at max {2}.";
    
    // Appointments

    public const string CannotCreateAppointmentBecauseOfDependencies =
        "Can not create an appointment because you dont have needed health certificates or appointments to receive needed health certificates yet";
}   