using SurveyBasket.Api.Contracts.Users;
using System.Net.WebSockets;

namespace SurveyBasket.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;


    public async Task<Result<UserProfileResponse>> GetProfileInfoAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _userManager.Users.Where(x => x.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .AsNoTracking()
            .SingleAsync(cancellationToken);

        return Result.Success(response);
    }
    public async Task UpdateProfileInfoAsync(string userId, UpdateProfileRequest request)
    {
        //var user = await _userManager.FindByIdAsync(userId);

        //user = request.Adapt(user);

        //await _userManager.UpdateAsync(user!);

        await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                 setters
                   .SetProperty(user => user.FirstName, request.FirstName)
                   .SetProperty(user => user.LastName, request.LastName)
            );
    }
    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
