using Dapper;
using FarmaciaSystem.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly string TableName;
        protected readonly string[] SelectColumns;

        protected BaseRepository(string tableName, string[] selectColumns)
        {
            TableName = tableName;
            SelectColumns = selectColumns;
        }

        protected async Task<SQLiteConnection> GetConnectionAsync()
        {
            var connection = new SQLiteConnection(DatabaseConfig.ConnectionString);
            await connection.OpenAsync();
            return connection;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $"SELECT {string.Join(", ", SelectColumns)} FROM {TableName} WHERE Id = @id";
                return await connection.QueryFirstOrDefaultAsync<T>(sql, new { id });
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $"SELECT {string.Join(", ", SelectColumns)} FROM {TableName} ORDER BY Id";
                return await connection.QueryAsync<T>(sql);
            }
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            using (var connection = await GetConnectionAsync())
            {
                var offset = (page - 1) * pageSize;
                var sql = $"SELECT {string.Join(", ", SelectColumns)} FROM {TableName} ORDER BY Id LIMIT @pageSize OFFSET @offset";
                return await connection.QueryAsync<T>(sql, new { pageSize, offset });
            }
        }

        public virtual async Task<int> CountAsync()
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $"SELECT COUNT(*) FROM {TableName}";
                return await connection.QuerySingleAsync<int>(sql);
            }
        }

        public abstract Task<T> AddAsync(T entity);
        public abstract Task<T> UpdateAsync(T entity);
        public abstract Task<bool> DeleteAsync(int id);
        public abstract Task<IEnumerable<T>> SearchAsync(string searchTerm);
    }
}
