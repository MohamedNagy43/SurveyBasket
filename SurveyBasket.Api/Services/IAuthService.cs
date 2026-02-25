
using OneOf;
using SurveyBasket.Api.Abstractions;

namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string email, string password,CancellationToken cancellationToken = default); 
    Task<OneOf<AuthResponse,Error>> GetRefreshTokenAsync(string token, string refreshToken,CancellationToken cancellationToken = default);
    Task<OneOf<AuthResponse, Error>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
}
