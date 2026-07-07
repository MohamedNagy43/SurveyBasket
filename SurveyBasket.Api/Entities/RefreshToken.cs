using System.Security.Cryptography;

namespace SurveyBasket.Api.Entities;

public class RefreshToken
{
    public string Token { get; set; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => !IsExpired && !RevokedOn.HasValue;

    public RefreshToken(int expirationInDays)
    {
        ExpiresOn = CreatedOn.AddDays(expirationInDays);
    }
    public RefreshToken()
    {

    }
    public void Revoke()
    {
        RevokedOn = DateTime.UtcNow;
    }

}
