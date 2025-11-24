using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace UIABank.BC.Modelos
{
    public class ProveedorServicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }          
        public string Codigo { get; set; }          
        public int MinLongitudContrato { get; set; }
        public int MaxLongitudContrato { get; set; } 
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

