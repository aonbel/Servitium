using Domain.Entities.Core;

namespace Domain.Abstractions.RefreshToken;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public required int UserId { get; set; }
    public required DateTime ExpiresOn { get; set; }
}