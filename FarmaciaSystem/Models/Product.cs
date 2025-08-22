using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public int? ActiveIngredientId { get; set; }
        public string Concentration { get; set; }
        public int PharmaceuticalFormId { get; set; }
        public int? CategoryId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public bool RequiresPrescription { get; set; } = false;
        public int MinStock { get; set; } = 10;
        public int MaxStock { get; set; } = 1000;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public ActiveIngredient ActiveIngredient { get; set; }
        public PharmaceuticalForm PharmaceuticalForm { get; set; }
        public Category Category { get; set; }
        public List<Batch> Batches { get; set; } = new List<Batch>();
    }
}
