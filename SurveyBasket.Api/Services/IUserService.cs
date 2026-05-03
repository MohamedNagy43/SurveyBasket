using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId, CancellationToken cancellationToken = default);
    Task UpdateProfileInfoAsync(string userId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
