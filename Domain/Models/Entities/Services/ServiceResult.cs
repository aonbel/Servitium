using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class ServiceResult : Entity
{
    public required DateOnlySegment ActivePeriod { get; set; }   
}