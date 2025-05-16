namespace Domain.Abstractions.Result.Errors;

public static class HealthCertificateTemplateErrors
{
    public static Error NotFoundById(int id) =>
        new Error("HealthCertificateTemplateNotFoundById",
            $"Health certificate template with given id {id} does not exist");
}