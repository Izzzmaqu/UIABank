using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BC.Cuentas
{
    public interface ICuentaRepository
    {
        Task<int> ContarCuentasPorClienteTipoMonedaAsync(Guid clienteId, TipoCuenta tipo, Moneda moneda);
        Task<bool> ExisteNumeroCuentaAsync(string numero);
        Task AgregarAsync(Cuenta cuenta);
        Task<Cuenta?> ObtenerPorIdAsync(Guid id);
        Task<List<Cuenta>> ObtenerPorClienteAsync(Guid clienteId);
        Task<List<Cuenta>> BuscarAsync(
            Guid? clienteId,
            TipoCuenta? tipo,
            Moneda? moneda,
            EstadoCuenta? estado);

        Task GuardarCambiosAsync();
    }
}
