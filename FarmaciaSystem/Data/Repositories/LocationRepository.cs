using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class LocationRepository : BaseRepository<Location>
    {
        public LocationRepository() : base("Locations", new[]
        {
            "Id", "WarehouseId", "Code", "Description", "IsActive"
        })
        { }

        public override async Task<Location> AddAsync(Location location)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Locations (WarehouseId, Code, Description)
                    VALUES (@WarehouseId, @Code, @Description);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, location);
                location.Id = id;
                return location;
            }
        }

        public override async Task<Location> UpdateAsync(Location location)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Locations SET 
                        Code = @Code,
                        Description = @Description
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, location);
                return location;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Locations SET IsActive = 0 WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<Location>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT l.*, w.Name as WarehouseName
                    FROM Locations l
                    INNER JOIN Warehouses w ON l.WarehouseId = w.Id
                    WHERE (l.Code LIKE @term OR l.Description LIKE @term OR w.Name LIKE @term)
                    AND l.IsActive = 1
                    ORDER BY w.Name, l.Code";

                return await connection.QueryAsync<Location>(sql, new { term = $"%{searchTerm}%" });
            }
        }

        public async Task<IEnumerable<Location>> GetLocationsByWarehouseAsync(int warehouseId)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE WarehouseId = @warehouseId AND IsActive = 1
                    ORDER BY Code";

                return await connection.QueryAsync<Location>(sql, new { warehouseId });
            }
        }
    }
}
