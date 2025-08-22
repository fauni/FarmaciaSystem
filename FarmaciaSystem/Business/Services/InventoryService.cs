using FarmaciaSystem.Business.Validators;
using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Business.Services
{
    public class InventoryService
    {
        private readonly InventoryRepository _inventoryRepository;
        private readonly MovementRepository _movementRepository;
        private readonly ProductRepository _productRepository;
        private readonly InventoryValidator _validator;

        public InventoryService(
            InventoryRepository inventoryRepository,
            MovementRepository movementRepository,
            ProductRepository productRepository,
            InventoryValidator validator)
        {
            _inventoryRepository = inventoryRepository;
            _movementRepository = movementRepository;
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<bool> ProcessEntryAsync(Batch batch, string username, string reference)
        {
            try
            {
                await _validator.ValidateBatchAsync(batch);

                // Verificar si ya existe un lote con el mismo número en la misma ubicación
                var existingBatch = await GetExistingBatchAsync(batch.ProductId, batch.BatchNumber, batch.LocationId);

                if (existingBatch != null)
                {
                    // Actualizar stock existente
                    existingBatch.CurrentStock += batch.CurrentStock;
                    await _inventoryRepository.UpdateAsync(existingBatch);
                    batch = existingBatch;
                }
                else
                {
                    // Crear nuevo lote
                    batch = await _inventoryRepository.AddAsync(batch);
                }

                // Registrar movimiento
                var movement = new InventoryMovement
                {
                    BatchId = batch.Id,
                    Type = MovementType.Entry,
                    Quantity = batch.CurrentStock,
                    Reason = "Entrada de mercancía",
                    User = username,
                    Reference = reference
                };

                await _movementRepository.AddAsync(movement);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ProcessExitAsync(int productId, int warehouseId, int quantity, string reason, string username)
        {
            try
            {
                var batches = await GetAvailableBatchesForExitAsync(productId, warehouseId);
                var remainingQuantity = quantity;

                foreach (var batch in batches)
                {
                    if (remainingQuantity <= 0) break;

                    var quantityToDeduct = Math.Min(batch.CurrentStock, remainingQuantity);

                    // Actualizar stock del lote
                    batch.CurrentStock -= quantityToDeduct;
                    await _inventoryRepository.UpdateAsync(batch);

                    // Registrar movimiento
                    var movement = new InventoryMovement
                    {
                        BatchId = batch.Id,
                        Type = MovementType.Exit,
                        Quantity = quantityToDeduct,
                        Reason = reason,
                        User = username
                    };

                    await _movementRepository.AddAsync(movement);
                    remainingQuantity -= quantityToDeduct;
                }

                return remainingQuantity == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ProcessStockAdjustmentAsync(int batchId, int newStock, string reason, string username)
        {
            try
            {
                var batch = await _inventoryRepository.GetByIdAsync(batchId);
                if (batch == null) return false;

                var oldStock = batch.CurrentStock;
                var difference = newStock - oldStock;

                batch.CurrentStock = newStock;
                await _inventoryRepository.UpdateAsync(batch);

                // Registrar movimiento
                var movement = new InventoryMovement
                {
                    BatchId = batchId,
                    Type = MovementType.Adjustment,
                    Quantity = Math.Abs(difference),
                    Reason = $"{reason} (Ajuste: {oldStock} → {newStock})",
                    User = username
                };

                await _movementRepository.AddAsync(movement);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ProcessTransferAsync(int productId, int originWarehouseId, int destinationWarehouseId, int destinationLocationId, int quantity, string reason, string username)
        {
            try
            {
                var batches = await GetAvailableBatchesForExitAsync(productId, originWarehouseId);
                var remainingQuantity = quantity;

                foreach (var batch in batches)
                {
                    if (remainingQuantity <= 0) break;

                    var quantityToTransfer = Math.Min(batch.CurrentStock, remainingQuantity);

                    // Reducir stock del lote origen
                    batch.CurrentStock -= quantityToTransfer;
                    await _inventoryRepository.UpdateAsync(batch);

                    // Registrar salida del origen
                    var exitMovement = new InventoryMovement
                    {
                        BatchId = batch.Id,
                        Type = MovementType.Transfer,
                        Quantity = quantityToTransfer,
                        Reason = $"Transferencia a almacén {destinationWarehouseId}: {reason}",
                        User = username,
                        DestinationWarehouseId = destinationWarehouseId
                    };

                    await _movementRepository.AddAsync(exitMovement);

                    // Crear o actualizar lote en destino
                    var destinationBatch = new Batch
                    {
                        ProductId = batch.ProductId,
                        WarehouseId = destinationWarehouseId,
                        LocationId = destinationLocationId,
                        BatchNumber = batch.BatchNumber,
                        ExpiryDate = batch.ExpiryDate,
                        CurrentStock = quantityToTransfer,
                        PurchasePrice = batch.PurchasePrice,
                        SupplierId = batch.SupplierId
                    };

                    var existingDestinationBatch = await GetExistingBatchAsync(batch.ProductId, batch.BatchNumber, destinationLocationId);

                    if (existingDestinationBatch != null)
                    {
                        existingDestinationBatch.CurrentStock += quantityToTransfer;
                        await _inventoryRepository.UpdateAsync(existingDestinationBatch);
                        destinationBatch = existingDestinationBatch;
                    }
                    else
                    {
                        destinationBatch = await _inventoryRepository.AddAsync(destinationBatch);
                    }

                    // Registrar entrada en destino
                    var entryMovement = new InventoryMovement
                    {
                        BatchId = destinationBatch.Id,
                        Type = MovementType.Entry,
                        Quantity = quantityToTransfer,
                        Reason = $"Transferencia desde almacén {originWarehouseId}: {reason}",
                        User = username
                    };

                    await _movementRepository.AddAsync(entryMovement);
                    remainingQuantity -= quantityToTransfer;
                }

                return remainingQuantity == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Batch>> GetExpiringBatchesAsync(int daysAhead = 30)
        {
            return await _inventoryRepository.GetExpiringBatchesAsync(daysAhead);
        }

        public async Task<IEnumerable<Batch>> GetBatchesByProductAsync(int productId)
        {
            return await _inventoryRepository.GetBatchesByProductAsync(productId);
        }

        public async Task<int> GetTotalStockByProductAsync(int productId)
        {
            return await _inventoryRepository.GetTotalStockByProductAsync(productId);
        }

        public async Task<IEnumerable<InventoryMovement>> GetMovementHistoryAsync(DateTime startDate, DateTime endDate)
        {
            return await _movementRepository.GetMovementsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<InventoryMovement>> GetMovementsByUserAsync(string username)
        {
            return await _movementRepository.GetMovementsByUserAsync(username);
        }

        public async Task<bool> MarkBatchAsExpiredAsync(int batchId, string username)
        {
            try
            {
                var batch = await _inventoryRepository.GetByIdAsync(batchId);
                if (batch == null) return false;

                var expiredQuantity = batch.CurrentStock;

                // Marcar como sin stock
                batch.CurrentStock = 0;
                await _inventoryRepository.UpdateAsync(batch);

                // Registrar movimiento de producto vencido
                var movement = new InventoryMovement
                {
                    BatchId = batchId,
                    Type = MovementType.Expired,
                    Quantity = expiredQuantity,
                    Reason = "Producto vencido - Retiro automático",
                    User = username
                };

                await _movementRepository.AddAsync(movement);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<Batch> GetExistingBatchAsync(int productId, string batchNumber, int locationId)
        {
            var batches = await _inventoryRepository.GetBatchesByProductAsync(productId);
            return batches.FirstOrDefault(b => b.BatchNumber == batchNumber && b.LocationId == locationId);
        }

        private async Task<IEnumerable<Batch>> GetAvailableBatchesForExitAsync(int productId, int warehouseId)
        {
            var batches = await _inventoryRepository.GetBatchesByProductAsync(productId);
            return batches.Where(b => b.WarehouseId == warehouseId && b.CurrentStock > 0)
                         .OrderBy(b => b.ExpiryDate); // FIFO - First In, First Out
        }
    }
}
