using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class SupplierRepository : BaseRepository<Supplier>
    {
        public SupplierRepository() : base("Suppliers", new[]
        {
            "Id", "Name", "Phone", "Email"
        })
        { }

        public override async Task<Supplier> AddAsync(Supplier supplier)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Suppliers (Name, Phone, Email)
                    VALUES (@Name, @Phone, @Email);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, supplier);
                supplier.Id = id;
                return supplier;
            }
        }

        public override async Task<Supplier> UpdateAsync(Supplier supplier)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Suppliers SET 
                        Name = @Name,
                        Phone = @Phone,
                        Email = @Email
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, supplier);
                return supplier;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                // Verificar si hay lotes asociados
                var batchCount = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Batches WHERE SupplierId = @id",
                    new { id });

                if (batchCount > 0)
                    throw new InvalidOperationException("No se puede eliminar el proveedor porque tiene lotes asociados");

                var sql = "DELETE FROM Suppliers WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<Supplier>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE Name LIKE @term OR Phone LIKE @term OR Email LIKE @term
                    ORDER BY Name";

                return await connection.QueryAsync<Supplier>(sql, new { term = $"%{searchTerm}%" });
            }
        }
    }
}
