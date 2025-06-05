using MiniAccountManagementSystem.Models.Identity;

namespace MiniAccountManagementSystem.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> CreateAsync(User user);
        Task<User?> GetByIdAsync(int id);
    }
}
