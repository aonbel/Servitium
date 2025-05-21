using Domain.Entities.Core;

namespace Domain.Entities.Services;

public class HealthCertificateTemplate : Entity
{
    public TimeSpan ActivePeriod { get; set; }
}