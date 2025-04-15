using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.Services;

public class HealthCertificateTemplate : Entity
{
    public TimeSpan ActivePeriod { get; set; }
}