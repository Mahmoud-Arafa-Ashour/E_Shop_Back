using E_Shop.Core.Abstractions;
using E_Shop.Models.Custumes.Contracts.Role;
namespace E_Shop.Service.IServices;
public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllRolesAsync(CancellationToken cancellationToken);
    Task<IEnumerable<RoleResponse>> GetActiveRoles(CancellationToken cancellationToken);
    Task<Result<RoleDetailsResponse>> AddRoleAsync(RoleRequest roleRequest, CancellationToken cancellationToken);
    Task<Result<RoleDetailsResponse>> GetRoleDetails(string RoleId, CancellationToken cancellationToken);
    Task<Result> UpdateAsync(string id, RoleRequest request);
    Task<Result> ToggleStatusAsync(string id);
    Task<Result<RoleResponse>> Get(string id , CancellationToken cancellationToken);
}
