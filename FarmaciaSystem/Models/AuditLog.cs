using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TableAffected { get; set; }
        public int? RecordId { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string IpAddress { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public User User { get; set; }
    }
}
