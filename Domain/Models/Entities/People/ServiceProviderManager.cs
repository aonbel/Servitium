using Domain.Models.Entities.Services;

namespace Domain.Models.Entities.People;

public class ServiceProviderManager : Person
{
    public required ServiceProvider ServiceProvider { get; set; }
}