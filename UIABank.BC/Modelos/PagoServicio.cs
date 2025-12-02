using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace UIABank.BC.Modelos
{
    public enum EstadoPagoServicio
    {
        Programado = 0,
        Pendiente = 1,     
        Exitoso = 2,
        Fallido = 3,
        Cancelado = 4,
        Rechazado = 5
    }

    public class PagoServicio
    {
        public int Id { get; set; }
        public string NumeroReferencia { get; set; }    
        public int ProveedorServicioId { get; set; }
        public ProveedorServicio ProveedorServicio { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public string NumeroContrato { get; set; }     
        public decimal Monto { get; set; }
        public string Moneda { get; set; }              
        public string CuentaOrigen { get; set; }        

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaProgramada { get; set; }  
        public DateTime? FechaEjecucion { get; set; }

        public EstadoPagoServicio Estado { get; set; }
        public decimal Comision { get; set; }
    }
}

