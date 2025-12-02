using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.DA
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObtenerPorIdAsync(int id);

        Task<Cliente?> ObtenerPorIdentificacionAsync(string identificacion);
        Task<bool> ExisteIdentificacionAsync(string identificacion);
        Task<Cliente> CrearAsync(Cliente cliente);
        Task<Cliente?> ActualizarAsync(Cliente cliente);
        Task<List<Cliente>> ObtenerTodosAsync();

        Task<List<Cliente>> ObtenerPorIdsAsync(List<int> idsClientes);
    }
}

