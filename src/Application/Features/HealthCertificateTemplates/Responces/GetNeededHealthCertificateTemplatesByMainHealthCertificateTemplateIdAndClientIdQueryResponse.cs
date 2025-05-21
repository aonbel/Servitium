namespace Application.Features.HealthCertificateTemplates.Responces;

public enum TypeOfRequirement
{
    AppointmentId,
    HealthCertificateTemplateId,
    HealthCertificateId
}

public record Requirement(TypeOfRequirement Type, int Id);

public record GetNeededHealthCertificateTemplatesByMainHealthCertificateTemplateIdAndClientIdQueryResponse(
    ICollection<Requirement> Requirements);
    