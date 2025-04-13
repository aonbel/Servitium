using Domain.Models.Entities.Core;
using Domain.Models.Entities.Services;

namespace Domain.Models.Entities.People;

public class Person : BaseEntity
{
    public required int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string MiddleName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
}