using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSystem.Models
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SessionToken { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public User User { get; set; }
    }
}
