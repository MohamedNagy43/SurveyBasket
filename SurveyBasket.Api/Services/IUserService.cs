using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string id);
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string id, CancellationToken cancellationToken = default);
    Task<Result> UnlockAsync(string id, CancellationToken cancellationToken = default);

    //profile
    Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId, CancellationToken cancellationToken = default);
    Task UpdateProfileInfoAsync(string userId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
