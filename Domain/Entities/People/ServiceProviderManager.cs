using Domain.Entities.Services;

namespace Domain.Entities.People;

public class ServiceProviderManager : Person
{
    public required ServiceProvider ServiceProvider { get; set; }
}