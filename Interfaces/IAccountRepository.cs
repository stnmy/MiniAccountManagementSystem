using MiniAccountManagementSystem.Dtos;

namespace MiniAccountManagementSystem.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<AccountTreeDto>> GetAllAsync();
        Task CreateAsync(AccountCreateDto input);
        Task<AccountEditDto> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task UpdateAsync(AccountEditDto input);
    }
}
