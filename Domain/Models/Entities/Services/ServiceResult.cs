using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class ServiceResult : Entity
{
    public (DateOnly begin, DateOnly end) ActivePeriod { get; set; }   
}