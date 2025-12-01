using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIABank.BC.Cuentas;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;

namespace UIABank.BW.CU
{
    public class CuentaService : ICuentaService
    {
        private readonly ICuentaRepository _cuentaRepository;

        public CuentaService(ICuentaRepository cuentaRepository)
        {
            _cuentaRepository = cuentaRepository;
        }

        public async Task<CuentaDto> AbrirCuentaAsync(AbrirCuentaRequest request)
        {
            // Regla: máximo 3 cuentas por tipo/moneda para el cliente
            var cantidad = await _cuentaRepository
                .ContarCuentasPorClienteTipoMonedaAsync(
                    request.ClienteId,
                    request.Tipo,
                    request.Moneda);

            if (cantidad >= 3)
                throw new InvalidOperationException(
                    "El cliente ya tiene el máximo de cuentas permitidas para ese tipo y moneda.");

            // Generar número de cuenta único de 12 dígitos
            string numero;
            var rnd = new Random();
            do
            {
                numero = rnd.Next(0, 999999999).ToString("D9") +
                         rnd.Next(0, 999).ToString("D3");
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

            return MapToDto(cuenta);
        }

        public async Task<IReadOnlyList<CuentaDto>> ObtenerCuentasPorClienteAsync(Guid clienteId)
        {
            var cuentas = await _cuentaRepository.ObtenerPorClienteAsync(clienteId);
            return cuentas.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<CuentaDto>> BuscarCuentasAsync(CuentasFiltroRequest filtro)
        {
            var cuentas = await _cuentaRepository.BuscarAsync(
                filtro.ClienteId,
                filtro.Tipo,
                filtro.Moneda,
                filtro.Estado);

            return cuentas.Select(MapToDto).ToList();
        }

        public async Task<CuentaDto?> ObtenerPorIdAsync(Guid cuentaId)
        {
            var cuenta = await _cuentaRepository.ObtenerPorIdAsync(cuentaId);
            return cuenta == null ? null : MapToDto(cuenta);
        }

        public async Task BloquearCuentaAsync(Guid cuentaId)
        {
            var cuenta = await _cuentaRepository.ObtenerPorIdAsync(cuentaId)
                         ?? throw new InvalidOperationException("La cuenta no existe.");

            cuenta.Bloquear();
            await _cuentaRepository.GuardarCambiosAsync();
        }

        public async Task CerrarCuentaAsync(Guid cuentaId)
        {
            var cuenta = await _cuentaRepository.ObtenerPorIdAsync(cuentaId)
                         ?? throw new InvalidOperationException("La cuenta no existe.");

            cuenta.Cerrar(); // La propia entidad valida que el saldo sea 0
            await _cuentaRepository.GuardarCambiosAsync();
        }

        private static CuentaDto MapToDto(Cuenta cuenta)
        {
            return new CuentaDto
            {
                Id = cuenta.Id,
                ClienteId = cuenta.ClienteId,
                Numero = cuenta.Numero,
                Tipo = cuenta.Tipo,
                Moneda = cuenta.Moneda,
                Saldo = cuenta.Saldo,
                Estado = cuenta.Estado
            };
        }
    }
}