using Dapper;
using Microsoft.Data.SqlClient;
using MiniAccountManagementSystem.Interfaces;
using MiniAccountManagementSystem.Models.Identity;
using System.Data;

namespace MiniAccountManagementSystem.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }
        public async Task<bool> CreateAsync(User user)
        {
            using var conn = new SqlConnection(_connectionString);

            var affectedRows = await conn.ExecuteAsync(
                "sp_CreateUser",
                new
                {
                    user.Username,
                    user.PasswordHash

                },
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
             );
            return result;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            var result = await conn.QueryFirstOrDefaultAsync<User>(
                "sp_GetUserByUsername",
                new { Username = username },
                commandType: CommandType.StoredProcedure
            );
            return result;
        }
    }
}

