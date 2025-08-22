using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class Location
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public Warehouse Warehouse { get; set; }
        public List<Batch> Batches { get; set; } = new List<Batch>();
    }
}
