using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Manager { get; set; }
        public string Phone { get; set; }
        public int? ManagerId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public User ManagerUser { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<Batch> Batches { get; set; } = new List<Batch>();
        public List<User> AssignedUsers { get; set; } = new List<User>();
    }
}
