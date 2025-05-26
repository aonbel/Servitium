namespace Servitium.Infrastructure.PagesConstants;

public static class ErrorMessages
{
    // User
    
    public const string UserName = "The {0} must be at least {2} and at max {1} characters long.";
    public const string UserFirstName = "The {0} must be at least {2} and at max {1} characters long.";
    public const string UserMiddleName = "The {0} must be at least {2} and at max {1} characters long.";
    public const string UserLastName = "The {0} must be at least {2} and at max {1} characters long.";
    public const string Password = "The {0} must be at least {2} and at max {1} characters long.";
    public const string ConfirmPassword = "The password and confirmation password do not match.";
    
    // Service provider
    
    public const string ServiceProviderName = "The {0} must be at least {2} and at max {1} characters long.";
    public const string ServiceProviderShortName = "The {0} must be at least {2} and at max {1} characters long.";
    public const string ServiceProviderAddress = "The {0} must be at least {2} and at max {1} characters long.";
    
    // Service
    
    public const string ServiceDuration = "The {0} must be at least {1} and at max {2}.";
    
    // Appointments

    public const string CannotCreateAppointmentBecauseOfDependencies =
        "Can not create an appointment because you dont have needed health certificates or appointments to receive needed health certificates yet";
    public const string CannotCreateAppointmentBecauseOfServiceNotBeingSelected =
        "Can not create an appointment because the service is not selected";
}   