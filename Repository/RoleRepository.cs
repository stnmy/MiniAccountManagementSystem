using Dapper;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Interfaces;
using MiniAccountManagementSystem.Models.Identity;
using System.Data;

namespace MiniAccountManagementSystem.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.ExecuteScalarAsync<bool>(
                "sp_IsUserInRole",
                new { UserId = userId, RoleName = roleName },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<string>(
                "sp_GetUserRoles",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, string roleName)
        {
            using var conn = new SqlConnection(_connectionString);
            var affectedRows = await conn.ExecuteAsync(
                "sp_AssignRoleToUser",
                new { UserId = userId, RoleName = roleName },
                commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, string roleName)
        {
            using var conn = new SqlConnection(_connectionString);
            var affectedRows = await conn.ExecuteAsync(
                "sp_RemoveRoleFromUser",
                new { UserId = userId, RoleName = roleName },
                commandType: CommandType.StoredProcedure);
            return affectedRows > 0;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<Role>(
                "sp_GetAllRoles",
                commandType: CommandType.StoredProcedure);
        }
    }
}
