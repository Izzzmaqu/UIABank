using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace UIABank.BW.Interfaces.BW
{
    using UIABank.BW.CU;

    public interface ICuentaService
    {
        Task<CuentaDto> AbrirCuentaAsync(AbrirCuentaRequest request);
        Task<IReadOnlyList<CuentaDto>> ObtenerCuentasPorClienteAsync(Guid clienteId);
        Task<IReadOnlyList<CuentaDto>> BuscarCuentasAsync(CuentasFiltroRequest filtro);
        Task<CuentaDto?> ObtenerPorIdAsync(Guid cuentaId);
        Task BloquearCuentaAsync(Guid cuentaId);
        Task CerrarCuentaAsync(Guid cuentaId);
    }
}