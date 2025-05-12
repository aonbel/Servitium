namespace Domain.Entities.Services;

public class HealthCertificate : HealthCertificateTemplate
{
    public required DateOnly ReceivingTime { get; set; }   
    
    public required string Description { get; set; }
    
    public required int TemplateId { get; set; }
}