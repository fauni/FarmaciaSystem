using Dapper;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Data.Repositories
{
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository() : base("Products", new[]
        {
            "Id", "Barcode", "Name", "ActiveIngredientId", "Concentration",
            "PharmaceuticalFormId", "CategoryId", "SalePrice", "PurchasePrice",
            "RequiresPrescription", "MinStock", "MaxStock", "IsActive", "CreatedDate"
        })
        { }

        public override async Task<Product> AddAsync(Product product)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Products (Barcode, Name, ActiveIngredientId, Concentration, PharmaceuticalFormId, 
                                        CategoryId, SalePrice, PurchasePrice, RequiresPrescription, MinStock, MaxStock)
                    VALUES (@Barcode, @Name, @ActiveIngredientId, @Concentration, @PharmaceuticalFormId,
                            @CategoryId, @SalePrice, @PurchasePrice, @RequiresPrescription, @MinStock, @MaxStock);
                    SELECT last_insert_rowid();";

                var id = await connection.QuerySingleAsync<int>(sql, product);
                product.Id = id;
                return product;
            }
        }

        public override async Task<Product> UpdateAsync(Product product)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    UPDATE Products SET 
                        Barcode = @Barcode,
                        Name = @Name,
                        ActiveIngredientId = @ActiveIngredientId,
                        Concentration = @Concentration,
                        PharmaceuticalFormId = @PharmaceuticalFormId,
                        CategoryId = @CategoryId,
                        SalePrice = @SalePrice,
                        PurchasePrice = @PurchasePrice,
                        RequiresPrescription = @RequiresPrescription,
                        MinStock = @MinStock,
                        MaxStock = @MaxStock
                    WHERE Id = @Id";

                await connection.ExecuteAsync(sql, product);
                return product;
            }
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = "UPDATE Products SET IsActive = 0 WHERE Id = @id";
                var rowsAffected = await connection.ExecuteAsync(sql, new { id });
                return rowsAffected > 0;
            }
        }

        public override async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT p.*, ai.Name as ActiveIngredientName, pf.Name as PharmaceuticalFormName, c.Name as CategoryName
                    FROM Products p
                    LEFT JOIN ActiveIngredients ai ON p.ActiveIngredientId = ai.Id
                    LEFT JOIN PharmaceuticalForms pf ON p.PharmaceuticalFormId = pf.Id
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    WHERE (p.Name LIKE @term OR p.Barcode LIKE @term OR ai.Name LIKE @term)
                    AND p.IsActive = 1
                    ORDER BY p.Name";

                return await connection.QueryAsync<Product>(sql, new { term = $"%{searchTerm}%" });
            }
        }

        public async Task<Product> GetByBarcodeAsync(string barcode)
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = $@"
                    SELECT {string.Join(", ", SelectColumns)} 
                    FROM {TableName} 
                    WHERE Barcode = @barcode AND IsActive = 1";

                return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { barcode });
            }
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT p.*, 
                           COALESCE(SUM(b.CurrentStock), 0) as TotalStock
                    FROM Products p
                    LEFT JOIN Batches b ON p.Id = b.ProductId AND b.IsActive = 1
                    WHERE p.IsActive = 1
                    GROUP BY p.Id
                    HAVING TotalStock <= p.MinStock
                    ORDER BY TotalStock ASC";

                return await connection.QueryAsync<Product>(sql);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsWithDetailsAsync()
        {
            using (var connection = await GetConnectionAsync())
            {
                var sql = @"
                    SELECT p.*, 
                           ai.Name as ActiveIngredientName,
                           pf.Name as PharmaceuticalFormName,
                           c.Name as CategoryName,
                           COALESCE(SUM(b.CurrentStock), 0) as TotalStock
                    FROM Products p
                    LEFT JOIN ActiveIngredients ai ON p.ActiveIngredientId = ai.Id
                    LEFT JOIN PharmaceuticalForms pf ON p.PharmaceuticalFormId = pf.Id
                    LEFT JOIN Categories c ON p.CategoryId = c.Id
                    LEFT JOIN Batches b ON p.Id = b.ProductId AND b.IsActive = 1
                    WHERE p.IsActive = 1
                    GROUP BY p.Id
                    ORDER BY p.Name";

                return await connection.QueryAsync<Product>(sql);
            }
        }
    }
}
