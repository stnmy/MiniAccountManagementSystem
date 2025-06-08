using MiniAccountManagementSystem.Models.Identity;

namespace MiniAccountManagementSystem.Interfaces
{
    public interface IRoleRepository
    {
        Task<bool> IsUserInRoleAsync(int userId, string roleName);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
