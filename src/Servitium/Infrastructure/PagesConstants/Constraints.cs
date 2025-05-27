namespace Servitium.Infrastructure.PagesConstants;

public static class Constraints
{
    // User
    
    public const int MinUsername = 3;
    public const int MaxUsername = 50;
    public const int MinFirstName = 3;
    public const int MaxFirstName = 100;
    public const int MinMiddleName  = 3;
    public const int MaxMiddleName = 100;
    public const int MinLastName = 3;
    public const int MaxLastName = 100;
    public const int MinPassword = 6;
    public const int MaxPassword = 100;
    
    // Service provider
    
    public const int MinServiceProviderName = 3;
    public const int MaxServiceProviderName = 50;
    public const int MinServiceProviderShortName = 3;
    public const int MaxServiceProviderShortName = 25;
    public const int MinServiceProviderAddress = 3;
    public const int MaxServiceProviderAddress = 75;
    
    // Service
    
    public const string MinServiceDuration = "00:01";
    public const string MaxServiceDuration = "23:59"; 
    
    // HealthCertificateTemplate
    
    public const int MinHealthCertificateTemplateName = 3;
    public const int MaxHealthCertificateTemplateName = 100;
    public const string MinHealthCertificateTemplateActivePeriod = "00:01:00";
    public const string MaxHealthCertificateTemplateActivePeriod = "17088:00:00";
    
    // Client
    
    public const string MinDateOfBirth = "01/01/1900";
}