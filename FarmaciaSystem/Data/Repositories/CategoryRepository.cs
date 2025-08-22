using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class CategoryRepository : BaseRepository<Category>
    {
        public CategoryRepository() : base("Categories", new[]
        {
            "Id", "Name", "Description"
        })
        { }

        public override async Task<Category> AddAsync(Category category)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Categories (Name, Description)
                    VALUES (@Name, @Description);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, category);
                category.Id = id;
                return category;
            }
        }

        public override async Task<Category> UpdateAsync(Category category)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Categories SET 
                        Name = @Name,
                        Description = @Description
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, category);
                return category;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                // Verificar si hay productos asociados
                var productCount = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Products WHERE CategoryId = @id AND IsActive = 1",
                    new { id });

                if (productCount > 0)
                    throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados");

                var sql = "DELETE FROM Categories WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<Category>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE Name LIKE @term OR Description LIKE @term
                    ORDER BY Name";

                return await connection.QueryAsync<Category>(sql, new { term = $"%{searchTerm}%" });
            }
        }
    }
}
