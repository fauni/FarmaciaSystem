using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
