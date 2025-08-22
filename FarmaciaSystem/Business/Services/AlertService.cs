using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Business.Services
{
    public class AlertService
    {
        private readonly ProductRepository _productRepository;
        private readonly InventoryRepository _inventoryRepository;

        public AlertService(ProductRepository productRepository, InventoryRepository inventoryRepository)
        {
            _productRepository = productRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<Alert>> GenerateAlertsAsync()
        {
            var alerts = new List<Alert>();

            // Generar alertas de stock bajo
            var lowStockAlerts = await GenerateLowStockAlertsAsync();
            alerts.AddRange(lowStockAlerts);

            // Generar alertas de productos próximos a vencer
            var expiryAlerts = await GenerateExpiryAlertsAsync();
            alerts.AddRange(expiryAlerts);

            return alerts;
        }

        private async Task<IEnumerable<Alert>> GenerateLowStockAlertsAsync()
        {
            var alerts = new List<Alert>();
            var lowStockProducts = await _productRepository.GetLowStockProductsAsync();

            foreach (var product in lowStockProducts)
            {
                var totalStock = await _inventoryRepository.GetTotalStockByProductAsync(product.Id);

                var priority = totalStock <= 0 ? AlertPriority.Critical :
                              totalStock <= product.MinStock * 0.5 ? AlertPriority.High : AlertPriority.Medium;

                var alert = new Alert
                {
                    Type = totalStock <= 0 ? AlertType.CriticalStock : AlertType.LowStock,
                    ProductId = product.Id,
                    Message = $"Stock {(totalStock <= 0 ? "crítico" : "bajo")}: {product.Name}. Stock actual: {totalStock}, Mínimo: {product.MinStock}",
                    Priority = priority
                };

                alerts.Add(alert);
            }

            return alerts;
        }

        private async Task<IEnumerable<Alert>> GenerateExpiryAlertsAsync()
        {
            var alerts = new List<Alert>();
            var expiringBatches = await _inventoryRepository.GetExpiringBatchesAsync(30);

            foreach (var batch in expiringBatches)
            {
                var daysToExpiry = (batch.ExpiryDate - DateTime.Now).Days;

                var priority = daysToExpiry <= 7 ? AlertPriority.Critical :
                              daysToExpiry <= 15 ? AlertPriority.High : AlertPriority.Medium;

                var alert = new Alert
                {
                    Type = daysToExpiry <= 0 ? AlertType.ExpiredProduct : AlertType.ExpiringProduct,
                    ProductId = batch.ProductId,
                    WarehouseId = batch.WarehouseId,
                    Message = $"Producto {(daysToExpiry <= 0 ? "vencido" : "próximo a vencer")}: Lote {batch.BatchNumber}, {Math.Abs(daysToExpiry)} días",
                    Priority = priority
                };

                alerts.Add(alert);
            }

            return alerts;
        }
    }
}
