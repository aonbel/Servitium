namespace Domain.Abstractions.Result.Errors;

public static class HealthCertificateTemplateErrors
{
    public static Error NotFoundById(int id) =>
        new Error("HealthCertificateTemplateNotFoundById",
            $"Health certificate template with given id {id} does not exist");
    
    public static Error NotFoundByName(string name) =>
        new Error("HealthCertificateTemplateNotFoundByName",
            $"Health certificate template with given name {name} does not exist");
    
    public static Error TemplateWithGivenNameAlreadyExists(string name) =>
        new Error("TemplateWithGivenNameAlreadyExists", $"Template with given name {name} already exists");
}