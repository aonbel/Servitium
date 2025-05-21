using Domain.Entities.Core;

namespace Domain.Entities.People;

public class Person : BaseEntity
{
    public required string UserId { get; set; }
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}