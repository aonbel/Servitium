using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class HealthCertificate : Entity
{
    public required DateOnlySegment ActivePeriod { get; set; }   
    
    public required string Description { get; set; }
}