using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class ProveedorServicioService : IProveedorServicioService
    {
        private readonly IProveedorServicioRepository _repo;

        public ProveedorServicioService(IProveedorServicioRepository repo)
        {
            _repo = repo;
        }

        public async Task<ProveedorServicio> CrearProveedorAsync(CrearProveedorServicioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del proveedor es obligatorio");

            if (dto.MinLongitudContrato <= 0 || dto.MaxLongitudContrato <= 0 ||
                dto.MinLongitudContrato > dto.MaxLongitudContrato)
                throw new ArgumentException("Rango de longitud de contrato inválido");

            if (await _repo.ExisteNombreAsync(dto.Nombre))
                throw new InvalidOperationException("Ya existe un proveedor con ese nombre");

            var proveedor = new ProveedorServicio
            {
                Nombre = dto.Nombre.Trim(),
                Codigo = dto.Codigo?.Trim(),
                MinLongitudContrato = dto.MinLongitudContrato,
                MaxLongitudContrato = dto.MaxLongitudContrato,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            return await _repo.CrearAsync(proveedor);
        }

        public async Task<ProveedorServicio> ActualizarProveedorAsync(ActualizarProveedorServicioDto dto)
        {
            var proveedor = await _repo.ObtenerPorIdAsync(dto.Id)
                ?? throw new ArgumentException("Proveedor no encontrado");

            if (dto.MinLongitudContrato <= 0 || dto.MaxLongitudContrato <= 0 ||
                dto.MinLongitudContrato > dto.MaxLongitudContrato)
                throw new ArgumentException("Rango de longitud de contrato inválido");

            proveedor.Nombre = dto.Nombre?.Trim() ?? proveedor.Nombre;
            proveedor.Codigo = dto.Codigo?.Trim();
            proveedor.MinLongitudContrato = dto.MinLongitudContrato;
            proveedor.MaxLongitudContrato = dto.MaxLongitudContrato;
            proveedor.Activo = dto.Activo;

            await _repo.ActualizarAsync(proveedor);
            return proveedor;
        }

        public Task<List<ProveedorServicio>> ObtenerTodosAsync()
            => _repo.ObtenerTodosAsync();

        public Task<ProveedorServicio> ObtenerPorIdAsync(int id)
            => _repo.ObtenerPorIdAsync(id);
    }
}
