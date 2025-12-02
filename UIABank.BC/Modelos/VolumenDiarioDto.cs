using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos.Reportes
{
    public class VolumenDiarioDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadTransacciones { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
