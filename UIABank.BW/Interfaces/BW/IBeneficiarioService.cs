using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BW.CU;

namespace UIABank.BW.Interfaces.BW
{
    public interface IBeneficiarioService
    {
        Task<BeneficiarioDto> RegistrarAsync(RegistrarBeneficiarioRequest request);
        Task ConfirmarAsync(Guid beneficiarioId, Guid clienteId);
        Task ActualizarAliasAsync(Guid beneficiarioId, ActualizarAliasBeneficiarioRequest request);
        Task EliminarAsync(Guid beneficiarioId, Guid clienteId);
        Task<IReadOnlyList<BeneficiarioDto>> ObtenerPorClienteAsync(BeneficiariosFiltroRequest filtro);
        Task<BeneficiarioDto?> ObtenerPorIdAsync(Guid beneficiarioId, Guid clienteId);
    }
}
