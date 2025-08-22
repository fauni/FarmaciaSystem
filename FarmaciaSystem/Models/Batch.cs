using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class Batch
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int LocationId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int CurrentStock { get; set; }
        public decimal PurchasePrice { get; set; }
        public int? SupplierId { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
        public Location Location { get; set; }
        public Supplier Supplier { get; set; }
        public List<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
    }
}
