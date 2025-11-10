using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BC.ReglasDeNegocio;

namespace UIABank.BW.CU
{
    public class PreCheckTransferenciaBW
    {
        private readonly ICuentaRepository _cuentaRepo;
        private readonly ITerceroRepository _terceroRepo;

        // Se inyectan los repositorios que permiten acceder a cuentas y terceros.
        public PreCheckTransferenciaBW(ICuentaRepository cuentaRepo, ITerceroRepository terceroRepo)
        {
            _cuentaRepo = cuentaRepo;
            _terceroRepo = terceroRepo;
        }

        // Método que realiza las validaciones previas a la ejecución de la transferencia.
        public async Task<string> PreCheckTransferenciaAsync(Transferencia transferencia)
        {
            // 1 Verificar que el monto sea mayor que cero.
            if (!ReglasTransferencia.ValidarMontoPositivo(transferencia.Monto))
                return " El monto debe ser mayor que cero.";

            // 2️⃣ Buscar la cuenta origen en base de datos.
            var cuentaOrigen = await _cuentaRepo.GetByIdAsync(transferencia.CuentaOrigenId);
            if (cuentaOrigen == null)
                return " La cuenta origen no existe.";

            // 3️⃣ Validar que la cuenta esté activa.
            if (!ReglasTransferencia.ValidarEstadoCuenta(cuentaOrigen.Estado))
                return " La cuenta origen no está activa.";

            // 4️⃣ Calcular la comisión (1%) y verificar que haya saldo suficiente.
            decimal comision = transferencia.Monto * 0.01m;
            bool saldoValido = ReglasTransferencia.ValidarSaldo(
                cuentaOrigen.Saldo, transferencia.Monto, comision);

            if (!saldoValido)
                return " Saldo insuficiente para cubrir el monto y la comisión.";

            // 5️⃣ Validar el límite diario (por ejemplo, ₡500,000).
            decimal limiteDiario = 500000m;
            if (transferencia.Monto > limiteDiario)
                return $" El monto supera el límite diario permitido (₡{limiteDiario:N0}).";

            // 6️⃣ Si es hacia un tercero, validar que esté confirmado.
            if (transferencia.TerceroId.HasValue)
            {
                var tercero = await _terceroRepo.GetByIdAsync(transferencia.TerceroId.Value);

                if (tercero == null || tercero.Estado != "Confirmado")
                    return " El tercero no existe o no está confirmado.";
            }

            // 7️⃣ Si todas las validaciones pasan, se calculan los valores finales.
            decimal totalDebitar = transferencia.Monto + comision;
            decimal saldoDespues = cuentaOrigen.Saldo - totalDebitar;

            // 8️⃣ Retornar mensaje con todos los cálculos para mostrar al usuario.
            string resultado = $" Transferencia válida.\n" +
                               $"Saldo antes: ₡{cuentaOrigen.Saldo:N2}\n" +
                               $"Comisión (1%): ₡{comision:N2}\n" +
                               $"Total a debitar: ₡{totalDebitar:N2}\n" +
                               $"Saldo después: ₡{saldoDespues:N2}";

            return resultado;
        }
    }
}
    
