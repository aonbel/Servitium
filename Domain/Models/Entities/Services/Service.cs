using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class Service : Entity
{
    public required string ShortName { get; set; }
    public required string Description { get; set; }
    public required ICollection<HealthCertificateTemplate> Requirements { get; set; }
    public required ICollection<HealthCertificateTemplate> Result { get; set; }
    public required float PricePerHourForMaterials { get; set; }
    public required float PricePerHourForEquipment { get; set; }
    public required TimeSpan Duration { get; set; }
}