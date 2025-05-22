namespace Domain.Abstractions.Result.Errors;

public static class HealthCertificateTemplateErrors
{
    public static Error NotFoundById(int id) =>
        new ("HealthCertificateTemplateNotFoundById",
            $"Health certificate template with given id {id} does not exist");
    
    public static Error NotFoundByName(string name) =>
        new ("HealthCertificateTemplateNotFoundByName",
            $"Health certificate template with given name {name} does not exist");
    
    public static Error TemplateWithGivenNameAlreadyExists(string name) =>
        new ("TemplateWithGivenNameAlreadyExists", $"Template with given name {name} already exists");
}