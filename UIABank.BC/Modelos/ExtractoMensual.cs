using System;
using System.Collections.Generic;

namespace UIABank.BC.Modelos
{
    public class ExtractoMensual
    {
        public int ClienteId { get; set; }
        public Guid CuentaId { get; set; }
        public string NumeroCuenta { get; set; }

        public int Anio { get; set; }
        public int Mes { get; set; }

        public decimal SaldoInicial { get; set; }
        public decimal SaldoFinal { get; set; }

        public decimal TotalDebitos { get; set; }
        public decimal TotalCreditos { get; set; }
        public decimal TotalComisiones { get; set; }

        public List<MovimientoHistorial> Movimientos { get; set; } = new();
    }
}

