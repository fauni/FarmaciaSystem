using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public int? AssignedWarehouseId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastAccess { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }

        // Propiedades de navegación
        public Role Role { get; set; }
        public Warehouse AssignedWarehouse { get; set; }
        public List<UserSession> Sessions { get; set; } = new List<UserSession>();
        public List<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
