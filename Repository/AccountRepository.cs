using Dapper;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Dtos;
using MiniAccountManagementSystem.Interfaces;
using System.Data;

namespace MiniAccountManagementSystem.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<AccountTreeDto>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryAsync<AccountTreeDto>(
                "sp_ManageChartOfAccounts",
                new { Action = "GETALL" },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task CreateAsync(AccountCreateDto input)
        {
            using var conn = new SqlConnection(_connectionString);
            var parameters = new
            {
                Action = "CREATE",
                Name = input.Name,
                ParentId = input.ParentId,
                Type = input.Type.ToString()
            };

            await conn.ExecuteAsync("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);

            var count = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM VoucherEntries WHERE AccountId = @AccountId",
                new { AccountId = id });

            if (count > 0)
            {
                throw new InvalidOperationException("Cannot delete this account because it has related voucher entries.");
            }

            await conn.ExecuteAsync(
                "sp_ManageChartOfAccounts",
                new { Action = "DELETE", Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(AccountEditDto input)
        {
            using var conn = new SqlConnection(_connectionString);
            var parameters = new
            {
                Action = "UPDATE",
                Id = input.Id,
                Name = input.Name,
                ParentId = input.ParentId,
                Type = input.Type.ToString()
            };

            await conn.ExecuteAsync("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<AccountEditDto> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryFirstOrDefaultAsync<AccountEditDto>(
                "sp_ManageChartOfAccounts",
                new { Action = "GETBYID", Id = id },
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}
