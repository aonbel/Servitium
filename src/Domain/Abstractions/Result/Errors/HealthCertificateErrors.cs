namespace Domain.Abstractions.Result.Errors;

public static class HealthCertificateErrors
{
    public static Error NotFoundByTemplateIdAmongHealthCertificatesOfClient(int clientId, int templateId) =>
        new Error("NotFoundByTemplateIdAmongHealthCertificatesOfClient",
            $"Health sertificate with given template id {templateId} does not exist among health certificates of user with given id {clientId}");
}