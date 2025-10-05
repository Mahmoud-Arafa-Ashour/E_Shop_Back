using E_Shop.Core.Abstractions;
using E_Shop.Models.Custumes.Contracts.UserRole;

namespace E_Shop.Service.IServices;

public interface IUserRoleService
{
    Task<Result> AssignRole(UserRoleRequest request, CancellationToken cancellationToken);
}
