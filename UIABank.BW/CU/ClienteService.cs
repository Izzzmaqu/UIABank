using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<Cliente> CrearClienteAsync(ClienteDto dto)
        {
            if (await _clienteRepository.ExisteIdentificacionAsync(dto.Identificacion))
            {
                throw new InvalidOperationException("La identificación ya está registrada");
            }

            if (string.IsNullOrWhiteSpace(dto.Identificacion))
                throw new ArgumentException("La identificación es requerida");

            if (string.IsNullOrWhiteSpace(dto.NombreCompleto))
                throw new ArgumentException("El nombre completo es requerido");

            if (string.IsNullOrWhiteSpace(dto.Telefono))
                throw new ArgumentException("El teléfono es requerido");

            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es requerido");

            var cliente = new Cliente
            {
                Identificacion = dto.Identificacion,
                NombreCompleto = dto.NombreCompleto,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                FechaCreacion = DateTime.UtcNow
            };

            return await _clienteRepository.CrearAsync(cliente);
        }

        public async Task<Cliente> ObtenerClientePorIdAsync(int id)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(id);
            if (cliente == null)
            {
                throw new KeyNotFoundException("Cliente no encontrado");
            }
            return cliente;
        }

        public async Task<Cliente> ActualizarClienteAsync(ActualizarClienteDto dto)
        {
            var cliente = await _clienteRepository.ObtenerPorIdAsync(dto.Id);
            if (cliente == null)
            {
                throw new KeyNotFoundException("Cliente no encontrado");
            }

            cliente.NombreCompleto = dto.NombreCompleto ?? cliente.NombreCompleto;
            cliente.Telefono = dto.Telefono ?? cliente.Telefono;
            cliente.Correo = dto.Correo ?? cliente.Correo;

            await _clienteRepository.ActualizarAsync(cliente);
            return cliente;
        }

        public async Task<List<Cliente>> ObtenerTodosClientesAsync()
        {
            return await _clienteRepository.ObtenerTodosAsync();
        }
    }
}

