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
    public class ProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly ProductValidator _validator;

        public ProductService(ProductRepository productRepository, ProductValidator validator)
        {
            _productRepository = productRepository;
            _validator = validator;
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _validator.ValidateAsync(product);
            return await _productRepository.AddAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            await _validator.ValidateAsync(product);
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            return await _productRepository.DeleteAsync(productId);
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<Product> GetProductByBarcodeAsync(string barcode)
        {
            return await _productRepository.GetByBarcodeAsync(barcode);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetProductsWithDetailsAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _productRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _productRepository.GetLowStockProductsAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedProductsAsync(int page, int pageSize)
        {
            return await _productRepository.GetPagedAsync(page, pageSize);
        }

        public async Task<int> GetProductCountAsync()
        {
            return await _productRepository.CountAsync();
        }
    }
}
