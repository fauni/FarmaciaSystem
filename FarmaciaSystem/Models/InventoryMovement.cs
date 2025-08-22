using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public enum MovementType
    {
        Entry,
        Exit,
        Adjustment,
        Expired,
        Return,
        Transfer
    }

    public class InventoryMovement
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public MovementType Type { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
        public string User { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.Now;
        public string Reference { get; set; }
        public int? DestinationWarehouseId { get; set; }

        // Propiedades de navegación
        public Batch Batch { get; set; }
        public Warehouse DestinationWarehouse { get; set; }
    }
}
