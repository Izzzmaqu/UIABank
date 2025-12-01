using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos.Reportes
{
    public class ClienteVolumenDto
    {
        public int ClienteId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public decimal VolumenTotal { get; set; }
    }
}
