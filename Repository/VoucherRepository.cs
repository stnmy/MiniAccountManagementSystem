using Dapper;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;
using System.Data;
using System.Security.Claims;

namespace MiniAccountManagementSystem.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly string _connectionString;
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VoucherRepository(
            IConfiguration configuration,
            IRoleRepository roleRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            return userId;
        }

        public async Task<IEnumerable<AccountVoucherDto>> GetLeafAccountsAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<AccountVoucherDto>(
                "sp_ManageChartOfAccounts",
                new { Action = "GETLEAF" },
                commandType: CommandType.StoredProcedure);
        }

        public async Task SaveVoucherAsync(VoucherDto voucher)
        {
            var userId = await GetCurrentUserIdAsync();
            var canSave = await _roleRepository.IsUserInRoleAsync(userId, "Admin") ||
                          await _roleRepository.IsUserInRoleAsync(userId, "Accountant");

            if (!canSave)
            {
                throw new UnauthorizedAccessException("You do not have permission to save vouchers.");
            }

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                var voucherEntriesTable = new DataTable();
                voucherEntriesTable.Columns.Add("AccountId", typeof(int));
                voucherEntriesTable.Columns.Add("Debit", typeof(decimal));
                voucherEntriesTable.Columns.Add("Credit", typeof(decimal));

                foreach (var entry in voucher.Entries)
                {
                    voucherEntriesTable.Rows.Add(entry.AccountId, entry.Debit, entry.Credit);
                }

                var parameters = new DynamicParameters();
                parameters.Add("@VoucherDate", voucher.Date);
                parameters.Add("@ReferenceNo", voucher.ReferenceNo);
                parameters.Add("@VoucherType", voucher.Type.ToString());
                parameters.Add("@VoucherEntries", voucherEntriesTable.AsTableValuedParameter("VoucherEntryType"));

                await conn.ExecuteAsync(
                    "sp_SaveVoucher",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
