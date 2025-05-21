using Domain.Entities.Core;
using Domain.Entities.Services;

namespace Domain.Entities.People;

public class Client : BaseEntity
{
    public required int PersonId { get; set; }
    public required DateOnly Birthday { get; set; }
    public required string Gender { get; set; }
}