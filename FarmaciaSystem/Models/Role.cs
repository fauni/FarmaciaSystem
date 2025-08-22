using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public List<User> Users { get; set; } = new List<User>();
        public List<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
