using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public enum AlertType
    {
        CriticalStock,
        LowStock,
        ExpiredProduct,
        ExpiringProduct
    }

    public enum AlertPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class Alert
    {
        public int Id { get; set; }
        public AlertType Type { get; set; }
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public string Message { get; set; }
        public AlertPriority Priority { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime AlertDate { get; set; } = DateTime.Now;
        public string TargetUser { get; set; }

        // Propiedades de navegación
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
