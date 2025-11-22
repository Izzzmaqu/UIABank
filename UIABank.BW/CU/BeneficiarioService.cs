using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UIABank.BC.Beneficiarios;
using UIABank.BW.Interfaces.BW;

namespace UIABank.BW.CU
{
    public class BeneficiarioService : IBeneficiarioService
    {
        private readonly IBeneficiarioRepository _beneficiarioRepository;

        public BeneficiarioService(IBeneficiarioRepository beneficiarioRepository)
        {
            _beneficiarioRepository = beneficiarioRepository;
        }

        public async Task<BeneficiarioDto> RegistrarAsync(RegistrarBeneficiarioRequest request)
        {
            // Regla: alias no repetido para el mismo cliente
            var aliasExiste = await _beneficiarioRepository
                .AliasExisteParaClienteAsync(request.ClienteId, request.Alias);

            if (aliasExiste)
                throw new InvalidOperationException(
                    "Ya existe un beneficiario con ese alias para este cliente.");

            var beneficiario = new Beneficiario(
                request.ClienteId,
                request.Alias,
                request.Banco,
                request.Moneda,
                request.NumeroCuenta,
                request.Pais);

            await _beneficiarioRepository.AgregarAsync(beneficiario);
            await _beneficiarioRepository.GuardarCambiosAsync();

            return MapToDto(beneficiario);
        }

        public async Task ConfirmarAsync(Guid beneficiarioId, Guid clienteId)
        {
            var beneficiario = await _beneficiarioRepository.ObtenerPorIdAsync(beneficiarioId)
                               ?? throw new InvalidOperationException("El beneficiario no existe.");

            if (beneficiario.ClienteId != clienteId)
                throw new InvalidOperationException("El beneficiario no pertenece al cliente indicado.");

            beneficiario.Confirmar();
            await _beneficiarioRepository.GuardarCambiosAsync();
        }

        public async Task ActualizarAliasAsync(Guid beneficiarioId, ActualizarAliasBeneficiarioRequest request)
        {
            var beneficiario = await _beneficiarioRepository.ObtenerPorIdAsync(beneficiarioId)
                               ?? throw new InvalidOperationException("El beneficiario no existe.");

            if (beneficiario.ClienteId != request.ClienteId)
                throw new InvalidOperationException("El beneficiario no pertenece al cliente indicado.");

            // Validar alias único para ese cliente
            var aliasExiste = await _beneficiarioRepository
                .AliasExisteParaClienteAsync(request.ClienteId, request.NuevoAlias);

            // Si el alias que encuentra es el mismo registro actual, no hay problema
            if (aliasExiste && !string.Equals(beneficiario.Alias, request.NuevoAlias.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Ya existe otro beneficiario con ese alias para este cliente.");
            }

            beneficiario.CambiarAlias(request.NuevoAlias);
            await _beneficiarioRepository.GuardarCambiosAsync();
        }

        public async Task EliminarAsync(Guid beneficiarioId, Guid clienteId)
        {
            var beneficiario = await _beneficiarioRepository.ObtenerPorIdAsync(beneficiarioId)
                               ?? throw new InvalidOperationException("El beneficiario no existe.");

            if (beneficiario.ClienteId != clienteId)
                throw new InvalidOperationException("El beneficiario no pertenece al cliente indicado.");

            // TODO: aquí debería validarse que no tenga operaciones programadas pendientes
            // cuando el módulo de Transferencias Programadas esté implementado.

            await _beneficiarioRepository.EliminarAsync(beneficiario);
            await _beneficiarioRepository.GuardarCambiosAsync();
        }

        public async Task<IReadOnlyList<BeneficiarioDto>> ObtenerPorClienteAsync(BeneficiariosFiltroRequest filtro)
        {
            var lista = await _beneficiarioRepository.ObtenerPorClienteAsync(
                filtro.ClienteId,
                filtro.Alias,
                filtro.Banco,
                filtro.Pais);

            return lista.Select(MapToDto).ToList();
        }

        public async Task<BeneficiarioDto?> ObtenerPorIdAsync(Guid beneficiarioId, Guid clienteId)
        {
            var beneficiario = await _beneficiarioRepository.ObtenerPorIdAsync(beneficiarioId);
            if (beneficiario == null || beneficiario.ClienteId != clienteId)
                return null;

            return MapToDto(beneficiario);
        }

        private static BeneficiarioDto MapToDto(Beneficiario b)
        {
            return new BeneficiarioDto
            {
                Id = b.Id,
                ClienteId = b.ClienteId,
                Alias = b.Alias,
                Banco = b.Banco,
                Moneda = b.Moneda,
                NumeroCuenta = b.NumeroCuenta,
                Pais = b.Pais,
                Estado = b.Estado
            };
        }
    }
}
