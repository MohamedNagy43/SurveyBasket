namespace SurveyBasket.Api.Entities;

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public bool IsActive => !IsExpired && !RevokedOn.HasValue;

    public void Revoke()
    {
        RevokedOn = DateTime.UtcNow;
    }

}
