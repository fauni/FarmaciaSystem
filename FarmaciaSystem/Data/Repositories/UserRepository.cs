using Dapper;
using FarmaciaSystem.Models;
using FarmaciaSystem.Utils.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository() : base("Users", new[]
        {
            "Id", "Username", "PasswordHash", "Email", "FullName", "Phone",
            "RoleId", "AssignedWarehouseId", "IsActive", "LastAccess",
            "CreatedDate", "ModifiedDate", "CreatedBy"
        })
        { }

        public override async Task<User> AddAsync(User user)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Users (Username, PasswordHash, Email, FullName, Phone, RoleId, AssignedWarehouseId, CreatedBy)
                    VALUES (@Username, @PasswordHash, @Email, @FullName, @Phone, @RoleId, @AssignedWarehouseId, @CreatedBy);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, new
                {
                    user.Username,
                    user.PasswordHash,
                    user.Email,
                    user.FullName,
                    user.Phone,
                    user.RoleId,
                    user.AssignedWarehouseId,
                    user.CreatedBy
                });

                user.Id = id;
                return user;
            }
        }

        public override async Task<User> UpdateAsync(User user)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Users SET 
                        Username = @Username,
                        Email = @Email,
                        FullName = @FullName,
                        Phone = @Phone,
                        RoleId = @RoleId,
                        AssignedWarehouseId = @AssignedWarehouseId,
                        IsActive = @IsActive,
                        ModifiedDate = datetime('now')
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, user);
                return user;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Users SET IsActive = 0 WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<User>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE (Username LIKE @term OR FullName LIKE @term OR Email LIKE @term)
                    AND IsActive = 1
                    ORDER BY FullName";

                return await connection.QueryAsync<User>(sql, new { term = $"%{searchTerm}%" });
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE Username = @username AND IsActive = 1";

                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { username });
            }
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                // Actualizar último acceso
                await UpdateLastAccessAsync(user.Id);
                return user;
            }
            return null;
        }

        public async Task UpdateLastAccessAsync(int userId)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Users SET LastAccess = datetime('now') WHERE Id = @userId";
                await connection.ExecuteAsync(sql, new { userId });
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            using (var connection = await GetConnectionAsync())
            {
                var passwordHash = PasswordHelper.HashPassword(newPassword);
                var sql = @"
                    UPDATE Users SET 
                        PasswordHash = @passwordHash,
                        ModifiedDate = datetime('now')
                    WHERE Id = @userId";

                var rowsAffected = await connection.ExecuteAsync(sql, new { userId, passwordHash });
                return rowsAffected > 0;
            }
        }

        public async Task<User> GetUserWithRoleAsync(int userId)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT u.*, r.Name as RoleName, r.Description as RoleDescription
                    FROM Users u
                    INNER JOIN Roles r ON u.RoleId = r.Id
                    WHERE u.Id = @userId AND u.IsActive = 1";

                return await connection.QueryFirstOrDefaultAsync<User>(sql, new { userId });
            }
        }
    }
}
