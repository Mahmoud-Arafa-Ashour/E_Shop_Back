using E_Shop.Core.Abstractions;
using E_Shop.Models.Custumes.Contracts.User;

namespace E_Shop.Service.IServices;

public interface IUserServices
{
    Task<Result<UserProfileResponse>> GetUserInfo(string userid);
    Task<Result> UpdateProfile(string userid, UpdateProfileRequest request);
    Task<Result> ChangePassword(string userid, ChangePasswordRequset requset);
    Task<Result<UserResponse>> GetUser(string Id, CancellationToken cancellationToken);
    Task<List<UserResponse>> GetAllUsers(CancellationToken cancellationToken);
}
