using FarmaciaSystem.Data.Repositories;
using FarmaciaSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Business.Validators
{
    public class ProductValidator
    {
        private readonly ProductRepository _productRepository;

        public ProductValidator(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task ValidateAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("El nombre del producto es requerido");

            if (product.Name.Length > 200)
                throw new ArgumentException("El nombre del producto no puede exceder 200 caracteres");

            if (product.SalePrice <= 0)
                throw new ArgumentException("El precio de venta debe ser mayor a cero");

            if (product.PurchasePrice <= 0)
                throw new ArgumentException("El precio de compra debe ser mayor a cero");

            if (product.SalePrice <= product.PurchasePrice)
                throw new ArgumentException("El precio de venta debe ser mayor al precio de compra");

            if (product.MinStock < 0)
                throw new ArgumentException("El stock mínimo no puede ser negativo");

            if (product.MaxStock <= 0)
                throw new ArgumentException("El stock máximo debe ser mayor a cero");

            if (product.MaxStock <= product.MinStock)
                throw new ArgumentException("El stock máximo debe ser mayor al stock mínimo");

            if (product.PharmaceuticalFormId <= 0)
                throw new ArgumentException("Debe seleccionar una forma farmacéutica válida");

            // Validar código de barras único (solo si se proporciona)
            if (!string.IsNullOrWhiteSpace(product.Barcode))
            {
                if (product.Barcode.Length < 8 || product.Barcode.Length > 50)
                    throw new ArgumentException("El código de barras debe tener entre 8 y 50 caracteres");

                var existingProduct = await _productRepository.GetByBarcodeAsync(product.Barcode);
                if (existingProduct != null && existingProduct.Id != product.Id)
                    throw new ArgumentException("Ya existe un producto con este código de barras");
            }

            // Validar concentración si hay principio activo
            if (product.ActiveIngredientId.HasValue && string.IsNullOrWhiteSpace(product.Concentration))
                throw new ArgumentException("Debe especificar la concentración cuando se selecciona un principio activo");
        }

        public void ValidateForDelete(Product product)
        {
            if (product == null)
                throw new ArgumentException("El producto no existe");

            // Aquí podrías agregar validaciones adicionales para eliminación
            // Por ejemplo, verificar si tiene stock, movimientos recientes, etc.
        }
    }
}
