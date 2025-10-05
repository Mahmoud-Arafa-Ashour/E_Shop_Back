using E_Shop.Core.Abstractions;
using E_Shop.Core.Authorization;
using E_Shop.Models.Custumes.Contracts.User;
using E_Shop.Service.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController(IUserServices userServices) : ControllerBase
{
    private readonly IUserServices _userServices = userServices;
    [HasPermission(Permissions.Info)]
    [HttpGet]
    public async Task<IActionResult> Info()
    {
        var result = await _userServices.GetUserInfo(User.GetUserID()!);
        return Ok(result);
    }
    [HasPermission(Permissions.UpdateInfo)]
    [HttpPut]
    public async Task<IActionResult> UpdateInfo(UpdateProfileRequest request)
    {
        var result = await _userServices.UpdateProfile(User.GetUserID()!, request);
        return NoContent();
    }
    [HasPermission(Permissions.ChangePassword)]
    [HttpPut]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequset requset)
    {
        var result = await _userServices.ChangePassword(User.GetUserID()!, requset);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpGet]
    public async Task<IActionResult> GetUser(string Id, CancellationToken cancellationToken)
    {
        var result = await _userServices.GetUser(Id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _userServices.GetAllUsers(cancellationToken);
        return Ok(result);
    }
}
