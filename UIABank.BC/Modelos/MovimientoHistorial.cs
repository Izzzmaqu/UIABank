using System;

namespace UIABank.BC.Modelos
{
    public class MovimientoHistorial
    {
        public DateTime Fecha { get; set; }
        public TipoMovimiento Tipo { get; set; }
        public string Descripcion { get; set; }
        public decimal Monto { get; set; }
        public decimal Comision { get; set; }
        public string Moneda { get; set; }
        public string Estado { get; set; }
        public string Referencia { get; set; }

        // Para poder filtrar por cuenta (Módulo F)
        public Guid? CuentaId { get; set; }
    }
}

