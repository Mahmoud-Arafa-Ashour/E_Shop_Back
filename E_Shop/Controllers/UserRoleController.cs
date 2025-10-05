using E_Shop.Models.Custumes.Contracts.UserRole;
using E_Shop.Service.IServices;
using Microsoft.AspNetCore.Mvc;
using E_Shop.Core.Abstractions;

namespace E_Shop.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserRoleController(IUserRoleService UserRoleService) : ControllerBase
{
    private readonly IUserRoleService _UserRoleServices = UserRoleService;
    [HttpPut]
    public async Task<IActionResult> AssignRole(UserRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await _UserRoleServices.AssignRole(request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
