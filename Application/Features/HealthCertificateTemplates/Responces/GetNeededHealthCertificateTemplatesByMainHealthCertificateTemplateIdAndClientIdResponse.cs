namespace Application.Features.HealthCertificateTemplates.Responces;

public enum TypeOfResult
{
    AppointmentId,
    HealthCertificateTemplateId,
    HealthCertificateId
}

public record GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdResponse(
    ICollection<(TypeOfResult type, int id)> Result);