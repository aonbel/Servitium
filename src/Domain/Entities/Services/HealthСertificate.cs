using Domain.Entities.Core;

namespace Domain.Entities.Services;

public class HealthCertificate : BaseEntity
{
    public required DateOnly ReceivingTime { get; set; }   
    
    public required string Description { get; set; }
    
    public required int TemplateId { get; set; }
    
    public required int ClientId { get; set; }
}