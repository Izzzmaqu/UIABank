using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Usuario { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Detalles { get; set; } = string.Empty;
        public string? Ip { get; set; }
    }
}
