using Domain.Models.Entities.Core;

namespace Domain.Models.Entities.People;

public class Person : BaseEntity
{
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}