using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIABank.BC.Modelos
{
     public class Cuenta
    {
     
        public int Id { get; set; }

        // Número visible de la cuenta (12 dígitos típicamente).
        public string NumeroCuenta { get; set; } = string.Empty;

        // Tipo de cuenta: Ahorros, Corriente, Inversión, etc.
        public string Tipo { get; set; } = string.Empty;

        // Moneda de la cuenta (CRC o USD).
        public string Moneda { get; set; } = "CRC";

        // Monto actual disponible en la cuenta.
        public decimal Saldo { get; set; }

        // Estado de la cuenta: Activa, Bloqueada, Cerrada.
        public string Estado { get; set; } = "Activa";

        // Cliente al que pertenece la cuenta.
        public int ClienteId { get; set; }
    }
}
//clase para probar el funcionamiento de transferencia, la usado debe usar la de modulo B, borrar cuando este lista