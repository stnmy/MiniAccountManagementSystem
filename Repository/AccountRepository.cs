using Dapper;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Interfaces;
using MiniAccountManagementSystem.Dtos;
using System.Data;
using System.Security.Claims;

namespace MiniAccountManagementSystem.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly string _connectionString;
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountRepository(
            IConfiguration configuration,
            IRoleRepository roleRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<int> GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId;
        }

        public async Task<IEnumerable<AccountTreeDto>> GetAllAsync()
        {
            var userId = await GetCurrentUserId();
            var canView = await _roleRepository.IsUserInRoleAsync(userId, "Admin") ||
                          await _roleRepository.IsUserInRoleAsync(userId, "Accountant") ||
                          await _roleRepository.IsUserInRoleAsync(userId, "Viewer");

            if (!canView)
            {
                throw new UnauthorizedAccessException("Insufficient permissions to view accounts");
            }

            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<AccountTreeDto>(
                "sp_ManageChartOfAccounts",
                new { Action = "GETALL" },
                commandType: CommandType.StoredProcedure);
        }

        public async Task CreateAsync(AccountCreateDto input)
        {
            var userId = await GetCurrentUserId();
            var canCreate = await _roleRepository.IsUserInRoleAsync(userId, "Admin") ||
                           await _roleRepository.IsUserInRoleAsync(userId, "Accountant");

            if (!canCreate)
            {
                throw new UnauthorizedAccessException("Insufficient permissions to create accounts");
            }

            using var conn = new SqlConnection(_connectionString);
            var parameters = new
            {
                Action = "CREATE",
                Name = input.Name,
                ParentId = input.ParentId,
                Type = input.Type.ToString(),
            };

            await conn.ExecuteAsync("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            var userId = await GetCurrentUserId();
            var isAdmin = await _roleRepository.IsUserInRoleAsync(userId, "Admin");

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can delete accounts");
            }

            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "sp_ManageChartOfAccounts",
                new { Action = "DELETE", Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(AccountEditDto input)
        {
            var userId = await GetCurrentUserId();
            var canEdit = await _roleRepository.IsUserInRoleAsync(userId, "Admin") ||
                          await _roleRepository.IsUserInRoleAsync(userId, "Accountant");

            if (!canEdit)
            {
                throw new UnauthorizedAccessException("Insufficient permissions to update accounts");
            }

            using var conn = new SqlConnection(_connectionString);
            var parameters = new
            {
                Action = "UPDATE",
                Id = input.Id,
                Name = input.Name,
                ParentId = input.ParentId,
                Type = input.Type.ToString(),
            };

            await conn.ExecuteAsync("sp_ManageChartOfAccounts", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<AccountEditDto> GetByIdAsync(int id)
        {
            var userId = await GetCurrentUserId();
            var canView = await _roleRepository.IsUserInRoleAsync(userId, "Admin") ||
                          await _roleRepository.IsUserInRoleAsync(userId, "Accountant") ||
                          await _roleRepository.IsUserInRoleAsync(userId, "Viewer");

            if (!canView)
            {
                throw new UnauthorizedAccessException("Insufficient permissions to view account details");
            }

            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryFirstOrDefaultAsync<AccountEditDto>(
                "sp_ManageChartOfAccounts",
                new { Action = "GETBYID", Id = id },
                commandType: CommandType.StoredProcedure);
        }
    }
}