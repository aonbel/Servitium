using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class Service : Entity
{
    public required string ShortName { get; set; }
    public required string Description { get; set; } 
    public required ICollection<HealthCertificate> Requirements { get; set; }
    public required ICollection<HealthCertificate> Result { get; set; }
    public required float PricePerHourForMaterials { get; set; }
    public required float PricePerHourForEquipment { get; set; }
    public required TimeSpan Duration { get; set; }
}