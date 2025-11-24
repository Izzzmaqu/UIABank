using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface IProveedorServicioRepository
    {
        Task<ProveedorServicio> CrearAsync(ProveedorServicio proveedor);
        Task<ProveedorServicio> ObtenerPorIdAsync(int id);
        Task<List<ProveedorServicio>> ObtenerTodosAsync();
        Task ActualizarAsync(ProveedorServicio proveedor);
        Task<bool> ExisteNombreAsync(string nombre);
    }
}
