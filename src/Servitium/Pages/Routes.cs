namespace Servitium.Pages;

internal static class Routes
{
    public const string Index = "/Index";
    public const string Privacy = "/Privacy";
    public const string SignIn = "/SignIn";
    public const string SignUp = "/SignUp";
    public const string LogOut = "/LogOut";
    
    public const string UsersIndex = "/Users/Index";
    
    public const string PersonsAdminIndex = "/Persons/Admin/Index";
    public const string PersonsManagerIndex = "/Persons/Manager/Index";
    
    public const string AppointmentsClientIndex = "/Appointments/Client/Index";
    public const string AppointmentClientCreate = "/Appointments/Client/Create";
    public const string AppointmentSpecialistIndex = "/Appointments/Specialist/Index";
    public const string AppointmentSpecialistDetails = "/Appointments/Specialist/Details";
    public const string AppointmentsAdminIndex = "/Appointments/Admin/Index";
    public const string AppointmentDetails = "/Appointments/Details";
    
    public const string ServiceProvidersIndex = "/ServiceProviders/Index";
    public const string ServiceProvidersAdminIndex = "/ServiceProviders/Admin/Index";
    public const string ServiceProviderDetails = "/ServiceProviders/Details";
    public const string ServiceProviderCreate = "/ServiceProviders/Create";
    
    public const string ClientCreate = "/Clients/Create";
    public const string ClientSpecialistDetails = "/Clients/Specialist/Details";
    
    public const string ServiceProviderManagerCreate = "/ServiceProviderManagers/Create";
    
    public const string SpecialistManagerCreate = "/Specialists/Manager/Create";
    
    public const string HealthCertificateTemplatesAdminIndex = "/HealthCertificateTemplates/Admin/Index";
    public const string HealthCertificateTemplateCreate = "/HealthCertificateTemplates/Create";
    public const string HealthCertificateTemplateGetAllRequiredFor = "/HealthCertificateTemplates/GetAllRequiredFor";
    
    public const string HealthCertificatesClientIndex = "/HealthCertificates/Client/Index";
    public const string HealthCertificateClientCreate = "/HealthCertificates/Client/Create";
    public const string HealthCertificatesSpecialistIndex = "/HealthCertificates/Specialist/Index";
    public const string HealthCertificateDetails = "/HealthCertificates/Details";
    
    public const string ServicesAdminIndex = "/Services/Admin/Index";
    public const string ServiceCreate = "/Services/Create";
}