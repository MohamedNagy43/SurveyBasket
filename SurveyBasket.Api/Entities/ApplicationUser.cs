namespace SurveyBasket.Api.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public bool IsDisabled { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
