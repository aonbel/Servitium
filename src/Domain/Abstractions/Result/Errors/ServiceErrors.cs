namespace Domain.Abstractions.Result.Errors;

public static class ServiceErrors
{
    public static Error NotFoundById(int id) =>
        new ("ServiceNotFoundById", $"Service with given id {id} does not exist");

    public static Error NotFoundByResultHealthCertificateTemplateId(int resultHealthCertificateTemplateId) =>
        new ("ServiceNotFoundByResultHealthCertificateTemplateId",
            $"Service with result health certificate template with id {resultHealthCertificateTemplateId} does not exist");
}