using MiniAccountManagementSystem.Dtos;

namespace MiniAccountManagementSystem.Interfaces
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<AccountVoucherDto>> GetLeafAccountsAsync();
        Task SaveVoucherAsync(VoucherDto voucher);
    }
}
