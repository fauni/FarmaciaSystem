using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class MovementRepository : BaseRepository<InventoryMovement>
    {
        public MovementRepository() : base("InventoryMovements", new[]
        {
            "Id", "BatchId", "Type", "Quantity", "Reason", "User",
            "MovementDate", "Reference", "DestinationWarehouseId"
        })
        { }

        public override async Task<InventoryMovement> AddAsync(InventoryMovement movement)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO InventoryMovements (BatchId, Type, Quantity, Reason, User, Reference, DestinationWarehouseId)
                    VALUES (@BatchId, @Type, @Quantity, @Reason, @User, @Reference, @DestinationWarehouseId);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, new
                {
                    movement.BatchId,
                    Type = (int)movement.Type,
                    movement.Quantity,
                    movement.Reason,
                    movement.User,
                    movement.Reference,
                    movement.DestinationWarehouseId
                });

                movement.Id = id;
                return movement;
            }
        }

        public override async Task<InventoryMovement> UpdateAsync(InventoryMovement movement)
        {
            throw new NotImplementedException("Los movimientos de inventario no se pueden modificar");
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException("Los movimientos de inventario no se pueden eliminar");
        }

        public override async Task<IEnumerable<InventoryMovement>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT im.*, p.Name as ProductName, b.BatchNumber
                    FROM InventoryMovements im
                    INNER JOIN Batches b ON im.BatchId = b.Id
                    INNER JOIN Products p ON b.ProductId = p.Id
                    WHERE (p.Name LIKE @term OR b.BatchNumber LIKE @term OR im.User LIKE @term)
                    ORDER BY im.MovementDate DESC";

                return await connection.QueryAsync<InventoryMovement>(sql, new { term = $"%{searchTerm}%" });
            }
        }

        public async Task<IEnumerable<InventoryMovement>> GetMovementsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT im.*, p.Name as ProductName, b.BatchNumber, w.Name as WarehouseName
                    FROM InventoryMovements im
                    INNER JOIN Batches b ON im.BatchId = b.Id
                    INNER JOIN Products p ON b.ProductId = p.Id
                    INNER JOIN Warehouses w ON b.WarehouseId = w.Id
                    WHERE date(im.MovementDate) BETWEEN date(@startDate) AND date(@endDate)
                    ORDER BY im.MovementDate DESC";

                return await connection.QueryAsync<InventoryMovement>(sql, new
                {
                    startDate = startDate.ToString("yyyy-MM-dd"),
                    endDate = endDate.ToString("yyyy-MM-dd")
                });
            }
        }

        public async Task<IEnumerable<InventoryMovement>> GetMovementsByUserAsync(string username)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT im.*, p.Name as ProductName, b.BatchNumber
                    FROM InventoryMovements im
                    INNER JOIN Batches b ON im.BatchId = b.Id
                    INNER JOIN Products p ON b.ProductId = p.Id
                    WHERE im.User = @username
                    ORDER BY im.MovementDate DESC";

                return await connection.QueryAsync<InventoryMovement>(sql, new { username });
            }
        }
    }
}
