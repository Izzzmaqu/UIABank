using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos  
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime? FechaBloqueo { get; set; }
        public int IntentosLogin { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int? ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }
    }
}

