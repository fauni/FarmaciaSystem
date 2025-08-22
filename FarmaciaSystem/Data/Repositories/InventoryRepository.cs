using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class InventoryRepository : BaseRepository<Batch>
    {
        public InventoryRepository() : base("Batches", new[]
        {
            "Id", "ProductId", "WarehouseId", "LocationId", "BatchNumber",
            "ExpiryDate", "CurrentStock", "PurchasePrice", "SupplierId",
            "EntryDate", "IsActive"
        })
        { }

        public override async Task<Batch> AddAsync(Batch batch)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Batches (ProductId, WarehouseId, LocationId, BatchNumber, ExpiryDate, 
                                       CurrentStock, PurchasePrice, SupplierId)
                    VALUES (@ProductId, @WarehouseId, @LocationId, @BatchNumber, @ExpiryDate,
                            @CurrentStock, @PurchasePrice, @SupplierId);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, batch);
                batch.Id = id;
                return batch;
            }
        }

        public override async Task<Batch> UpdateAsync(Batch batch)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Batches SET 
                        CurrentStock = @CurrentStock,
                        PurchasePrice = @PurchasePrice
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, batch);
                return batch;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Batches SET IsActive = 0 WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<Batch>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT b.*, p.Name as ProductName, w.Name as WarehouseName, l.Code as LocationCode
                    FROM Batches b
                    INNER JOIN Products p ON b.ProductId = p.Id
                    INNER JOIN Warehouses w ON b.WarehouseId = w.Id
                    INNER JOIN Locations l ON b.LocationId = l.Id
                    WHERE (p.Name LIKE @term OR b.BatchNumber LIKE @term)
                    AND b.IsActive = 1
                    ORDER BY b.ExpiryDate";

                return await connection.QueryAsync<Batch>(sql, new { term = $"%{searchTerm}%" });
            }
        }

        public async Task<IEnumerable<Batch>> GetExpiringBatchesAsync(int daysAhead = 30)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT b.*, p.Name as ProductName, p.Barcode, w.Name as WarehouseName, l.Code as LocationCode
                    FROM Batches b
                    INNER JOIN Products p ON b.ProductId = p.Id
                    INNER JOIN Warehouses w ON b.WarehouseId = w.Id
                    INNER JOIN Locations l ON b.LocationId = l.Id
                    WHERE date(b.ExpiryDate) <= date('now', '+' || @daysAhead || ' days')
                    AND b.CurrentStock > 0
                    AND b.IsActive = 1
                    ORDER BY b.ExpiryDate ASC";

                return await connection.QueryAsync<Batch>(sql, new { daysAhead });
            }
        }

        public async Task<IEnumerable<Batch>> GetBatchesByProductAsync(int productId)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT b.*, w.Name as WarehouseName, l.Code as LocationCode, l.Description as LocationDescription
                    FROM Batches b
                    INNER JOIN Warehouses w ON b.WarehouseId = w.Id
                    INNER JOIN Locations l ON b.LocationId = l.Id
                    WHERE b.ProductId = @productId
                    AND b.CurrentStock > 0
                    AND b.IsActive = 1
                    ORDER BY b.ExpiryDate ASC";

                return await connection.QueryAsync<Batch>(sql, new { productId });
            }
        }

        public async Task<bool> UpdateStockAsync(int batchId, int newStock)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Batches SET CurrentStock = @newStock WHERE Id = @batchId";
                var rowsAffected = await connection.ExecuteAsync(sql, new { batchId, newStock });
                return rowsAffected > 0;
            }
        }

        public async Task<int> GetTotalStockByProductAsync(int productId)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT COALESCE(SUM(CurrentStock), 0) 
                    FROM Batches 
                    WHERE ProductId = @productId AND IsActive = 1";

                return await connection.QuerySingleAsync<int>(sql, new { productId });
            }
        }
    }
}
