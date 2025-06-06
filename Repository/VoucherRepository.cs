using Dapper;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;
using System.Data;

namespace MiniAccountManagementSystem.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly string _connectionString;

        public VoucherRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
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
