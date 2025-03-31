using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class Service : Entity
{
    public required string ShortName { get; set; }
    public required string Description { get; set; } 
    public required ICollection<ServiceResult> Requirements { get; set; }
    public required ICollection<ServiceResult> Result { get; set; }
    public required float PricePerHourForMaterials { get; set; }
    public required float PricePerHourForEquipment { get; set; }
    public required float[] Prices { get; set; }
    public required (TimeOnly begin, TimeOnly end)[] Schedule { get; set; } 
}