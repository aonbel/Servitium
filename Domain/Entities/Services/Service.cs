using Domain.Entities.Core;

namespace Domain.Entities.Services;

public class Service : Entity
{
    public required string ShortName { get; set; }
    public required string Description { get; set; }
    public required ICollection<int> RequiredHealthCertificateTemplateIds { get; set; }
    public required ICollection<int> ResultHealthCertificateTemplateIds { get; set; }
    public required float PricePerHourForMaterials { get; set; }
    public required float PricePerHourForEquipment { get; set; }
    public required TimeSpan Duration { get; set; }
}