using System;
using UIABank.BC.Cuentas;

namespace UIABank.BW.Cuentas.DTOs
{
    public class AbrirCuentaRequest
    {
        public Guid ClienteId { get; set; }
        public TipoCuenta Tipo { get; set; }
        public Moneda Moneda { get; set; }
        public decimal SaldoInicial { get; set; }
    }
}
