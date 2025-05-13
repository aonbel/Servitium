using Domain.Entities.Core;

namespace Domain.Abstractions.RefreshToken;

public class RefreshToken : BaseEntity
{
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public required DateTime ExpiresOn { get; set; }
}