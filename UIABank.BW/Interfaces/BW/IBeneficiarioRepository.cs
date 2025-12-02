using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Cuentas;
using UIABank.BC.Modelos;

namespace UIABank.BC.Beneficiarios
{
    public interface IBeneficiarioRepository
    {
        Task<bool> AliasExisteParaClienteAsync(Guid clienteId, string alias);
        Task AgregarAsync(Beneficiario beneficiario);
        Task<Beneficiario?> ObtenerPorIdAsync(Guid id);
        Task<List<Beneficiario>> ObtenerPorClienteAsync(
            Guid clienteId,
            string? alias,
            string? banco,
            string? pais);

        Task EliminarAsync(Beneficiario beneficiario);
        Task GuardarCambiosAsync();
    }
}
