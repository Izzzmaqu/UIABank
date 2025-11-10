using System;

namespace UIABank.BC.ReglasDeNegocio
{
 
    public static class ReglasTransferencia
    {
        // Verifica que el saldo actual sea suficiente para cubrir
        // el monto de la transferencia más la comisión.
        public static bool ValidarSaldo(decimal saldoActual, decimal monto, decimal comision)
        {
            return saldoActual >= (monto + comision);
        }

        // Asegura que el monto a transferir sea positivo.
        public static bool ValidarMontoPositivo(decimal monto)
        {
            return monto > 0;
        }

        // Comprueba que la moneda de origen y destino sean iguales.
        // Evita transferencias entre distintas monedas
        public static bool ValidarMoneda(string origen, string destino)
        {
            return origen == destino;
        }

        // Valida que la cuenta de origen esté activa
        public static bool ValidarEstadoCuenta(string estado)
        {
            return estado.Equals("Activa", StringComparison.OrdinalIgnoreCase);
        }
    }
}
