using System;
using System.Threading.Tasks;
using UIABank.BC.Cuentas;
using UIABank.BW.Cuentas.DTOs;

namespace UIABank.BW.Cuentas.Servicios
{
    public interface ICuentaService
    {
        Task<Cuenta> AbrirCuentaAsync(AbrirCuentaRequest request);
        // Más métodos luego: Consultar, Bloquear, Cerrar...
    }

    public class CuentaService : ICuentaService
    {
        private readonly ICuentaRepository _cuentaRepository;

        public CuentaService(ICuentaRepository cuentaRepository)
        {
            _cuentaRepository = cuentaRepository;
        }

        public async Task<Cuenta> AbrirCuentaAsync(AbrirCuentaRequest request)
        {
            // Regla: saldo inicial >= 0 ya la valida la entidad Cuenta en el constructor

            // Regla: no más de 3 cuentas del mismo tipo y moneda para el cliente
            var cantidad = await _cuentaRepository
                .ContarCuentasPorClienteTipoMonedaAsync(
                    request.ClienteId,
                    request.Tipo,
                    request.Moneda);

            if (cantidad >= 3)
                throw new InvalidOperationException("El cliente ya tiene el máximo de cuentas permitidas para ese tipo y moneda.");

            // Generar número de cuenta único de 12 dígitos
            string numero;
            var rnd = new Random();
            do
            {
                numero = rnd.Next(0, 999999999).ToString("D9") + rnd.Next(0, 999).ToString("D3");
            }
            while (await _cuentaRepository.ExisteNumeroCuentaAsync(numero));

            var cuenta = new Cuenta(
                request.ClienteId,
                numero,
                request.Tipo,
                request.Moneda,
                request.SaldoInicial);

            await _cuentaRepository.AgregarAsync(cuenta);
            await _cuentaRepository.GuardarCambiosAsync();

            return cuenta;
        }
    }
}