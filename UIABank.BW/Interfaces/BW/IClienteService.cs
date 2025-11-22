using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UIABank.BC.Modelos;

namespace UIABank.BW.Interfaces.BW
{
    public class ClienteDto
    {
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
    }

    public class ActualizarClienteDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
    }

    public interface IClienteService
    {
        Task<Cliente> CrearClienteAsync(ClienteDto dto);
        Task<Cliente> ObtenerClientePorIdAsync(int id);
        Task<Cliente> ActualizarClienteAsync(ActualizarClienteDto dto);
        Task<List<Cliente>> ObtenerTodosClientesAsync();
    }
}

