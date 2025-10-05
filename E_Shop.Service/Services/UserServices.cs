using E_Shop.Core.Abstractions;
using E_Shop.Core.Persistent;
using E_Shop.Models.Custumes.Contracts.User;
using E_Shop.Service.IServices;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static E_Shop.Core.Abstractions.Errors;

namespace E_Shop.Service.Services;

public class UserServices(UserManager<ApplicationUser> userManager , RoleManager<ApplicationRole> RoleManager) : IUserServices
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _userRole = RoleManager;
    public async Task<Result<UserProfileResponse>> GetUserInfo(string userid)
    {
        var user = await _userManager.Users
            .Where(x => x.Id == userid)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();
        return Result.Success(user);
    }
    public async Task<Result> UpdateProfile(string userid, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userid);
        user = request.Adapt(user);
        await _userManager.UpdateAsync(user!);
        return Result.Success();
    }
    public async Task<Result> ChangePassword(string userid , ChangePasswordRequset requset)
    {
        var user = await _userManager.FindByIdAsync(userid);
        var result = await _userManager.ChangePasswordAsync(user!, requset.CurrentPassword, requset.NewPassword);
        if(result.Succeeded) 
            return Result.Success();
        var errors = result.Errors.First();
        return Result.Failure(new Error(errors.Code, errors.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result<UserResponse>> GetUser(string Id ,CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(Id);
        if (user is null)
            return Result.Failure<UserResponse>(UserErrors.NotFound);
        var roles = _userManager.GetRolesAsync(user);
        if(roles is null)
            return Result.Failure<UserResponse>(UserErrors.NotAssigned);
        var response = new UserResponse
        (user.Id,user.Email!,user.Name, user.Adress, user.PhoneNumber!,  roles.Result);
        response.Adapt<UserResponse>();
        return Result.Success(response);
    }

    public async Task<List<UserResponse>> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);

        var responseList = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var response = new UserResponse(
                user.Id,
                user.Email!,
                user.Name,
                user.Adress,
                user.PhoneNumber!,
                roles.ToList()
            );
            responseList.Add(response);
        }

        return responseList;
    }

}
