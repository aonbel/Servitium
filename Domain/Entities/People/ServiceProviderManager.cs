using Domain.Entities.Core;
using Domain.Entities.Services;

namespace Domain.Entities.People;

public class ServiceProviderManager : BaseEntity
{
    public required int PersonId { get; set; }
    public required ServiceProvider ServiceProvider { get; set; }
}