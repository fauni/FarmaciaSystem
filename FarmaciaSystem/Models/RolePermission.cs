using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
