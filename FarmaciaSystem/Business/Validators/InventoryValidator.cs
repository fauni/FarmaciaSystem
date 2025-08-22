using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Business.Validators
{
    public class InventoryValidator
    {
        public async Task ValidateBatchAsync(Batch batch)
        {
            if (batch.ProductId <= 0)
                throw new ArgumentException("Debe especificar un producto válido");

            if (batch.WarehouseId <= 0)
                throw new ArgumentException("Debe especificar un almacén válido");

            if (batch.LocationId <= 0)
                throw new ArgumentException("Debe especificar una ubicación válida");

            if (string.IsNullOrWhiteSpace(batch.BatchNumber))
                throw new ArgumentException("El número de lote es requerido");

            if (batch.BatchNumber.Length > 50)
                throw new ArgumentException("El número de lote no puede exceder 50 caracteres");

            if (batch.ExpiryDate <= DateTime.Now.Date)
                throw new ArgumentException("La fecha de vencimiento debe ser futura");

            if (batch.ExpiryDate > DateTime.Now.AddYears(10))
                throw new ArgumentException("La fecha de vencimiento no puede ser mayor a 10 años");

            if (batch.CurrentStock < 0)
                throw new ArgumentException("El stock no puede ser negativo");

            if (batch.CurrentStock > 999999)
                throw new ArgumentException("El stock no puede exceder 999,999 unidades");

            if (batch.PurchasePrice <= 0)
                throw new ArgumentException("El precio de compra debe ser mayor a cero");

            if (batch.PurchasePrice > 999999.99m)
                throw new ArgumentException("El precio de compra es demasiado alto");

            await Task.CompletedTask;
        }

        public void ValidateMovement(InventoryMovement movement)
        {
            if (movement.BatchId <= 0)
                throw new ArgumentException("Debe especificar un lote válido");

            if (movement.Quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero");

            if (movement.Quantity > 999999)
                throw new ArgumentException("La cantidad no puede exceder 999,999 unidades");

            if (string.IsNullOrWhiteSpace(movement.User))
                throw new ArgumentException("Debe especificar el usuario");

            if (movement.User.Length > 100)
                throw new ArgumentException("El nombre de usuario no puede exceder 100 caracteres");

            if (movement.Reason != null && movement.Reason.Length > 200)
                throw new ArgumentException("La razón no puede exceder 200 caracteres");

            if (movement.Reference != null && movement.Reference.Length > 100)
                throw new ArgumentException("La referencia no puede exceder 100 caracteres");
        }

        public void ValidateStockAdjustment(int batchId, int newStock, string reason)
        {
            if (batchId <= 0)
                throw new ArgumentException("Debe especificar un lote válido");

            if (newStock < 0)
                throw new ArgumentException("El nuevo stock no puede ser negativo");

            if (newStock > 999999)
                throw new ArgumentException("El nuevo stock no puede exceder 999,999 unidades");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Debe especificar la razón del ajuste");

            if (reason.Length > 200)
                throw new ArgumentException("La razón no puede exceder 200 caracteres");
        }

        public void ValidateTransfer(int productId, int originWarehouseId, int destinationWarehouseId, int quantity)
        {
            if (productId <= 0)
                throw new ArgumentException("Debe especificar un producto válido");

            if (originWarehouseId <= 0)
                throw new ArgumentException("Debe especificar un almacén de origen válido");

            if (destinationWarehouseId <= 0)
                throw new ArgumentException("Debe especificar un almacén de destino válido");

            if (originWarehouseId == destinationWarehouseId)
                throw new ArgumentException("El almacén de origen y destino no pueden ser el mismo");

            if (quantity <= 0)
                throw new ArgumentException("La cantidad a transferir debe ser mayor a cero");

            if (quantity > 999999)
                throw new ArgumentException("La cantidad a transferir no puede exceder 999,999 unidades");
        }
    }
}
