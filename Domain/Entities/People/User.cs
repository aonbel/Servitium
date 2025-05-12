using Domain.Entities.Core;

namespace Domain.Entities.People;

public class User : BaseEntity
{
    public required string Password { get; set; }
    public required string Username { get; init; }
    public required ICollection<string> Roles { get; set; }
}