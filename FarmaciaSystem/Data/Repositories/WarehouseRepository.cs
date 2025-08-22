using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class WarehouseRepository : BaseRepository<Warehouse>
    {
        public WarehouseRepository() : base("Warehouses", new[]
        {
            "Id", "Name", "Location", "Manager", "Phone", "ManagerId", "IsActive", "CreatedDate"
        })
        { }

        public override async Task<Warehouse> AddAsync(Warehouse warehouse)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Warehouses (Name, Location, Manager, Phone, ManagerId)
                    VALUES (@Name, @Location, @Manager, @Phone, @ManagerId);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, warehouse);
                warehouse.Id = id;
                return warehouse;
            }
        }

        public override async Task<Warehouse> UpdateAsync(Warehouse warehouse)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Warehouses SET 
                        Name = @Name,
                        Location = @Location,
                        Manager = @Manager,
                        Phone = @Phone,
                        ManagerId = @ManagerId
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, warehouse);
                return warehouse;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Warehouses SET IsActive = 0 WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<Warehouse>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE (Name LIKE @term OR Location LIKE @term OR Manager LIKE @term)
                    AND IsActive = 1
                    ORDER BY Name";

                return await connection.QueryAsync<Warehouse>(sql, new { term = $"%{searchTerm}%" });
            }
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync()
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE IsActive = 1
                    ORDER BY Name";

                return await connection.QueryAsync<Warehouse>(sql);
            }
        }
    }
}
