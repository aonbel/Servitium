using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.People;

public class User : BaseEntity
{
    public required string Password { get; set; }
    public required string Username { get; init; }
    public required string Role { get; set; }
}