using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UIABank.BC.Modelos;
using UIABank.BW.Interfaces.BW;
using UIABank.BW.Interfaces.DA;

namespace UIABank.BW.CU
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IJwtService _jwtService;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IClienteRepository clienteRepository,
            IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _jwtService = jwtService;
        }

        public async Task<Usuario> RegistrarUsuarioAsync(RegistroUsuarioDto dto)
        {
            if (await _usuarioRepository.ExisteEmailAsync(dto.Email))
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            }

            if (!EsEmailValido(dto.Email))
            {
                throw new ArgumentException("El correo electrónico no es válido");
            }

            if (!EsPasswordValido(dto.Password))
            {
                throw new ArgumentException(
                    "La contraseña debe tener mínimo 8 caracteres, incluir al menos 1 mayúscula, 1 número y 1 símbolo");
            }

            var rolesPermitidos = new[] { "Administrador", "Gestor", "Cliente" };
            if (!rolesPermitidos.Contains(dto.Rol))
            {
                throw new ArgumentException("Rol no válido");
            }

            if (dto.Rol == "Cliente")
            {
                if (!dto.ClienteId.HasValue)
                {
                    throw new ArgumentException("Un usuario con rol Cliente debe estar asociado a un cliente");
                }

                var cliente = await _clienteRepository.ObtenerPorIdAsync(dto.ClienteId.Value);
                if (cliente == null)
                {
                    throw new ArgumentException("El cliente especificado no existe");
                }

                if (cliente.Usuario != null)
                {
                    throw new InvalidOperationException("Este cliente ya tiene un usuario asociado");
                }
            }

            var usuario = new Usuario
            {
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                Rol = dto.Rol,
                Bloqueado = false,
                IntentosLogin = 0,
                FechaCreacion = DateTime.UtcNow,
                ClienteId = dto.ClienteId
            };

            return await _usuarioRepository.CrearAsync(usuario);
        }

        public async Task<LoginResultDto> AutenticarAsync(LoginDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(dto.Email);

            if (usuario == null)
            {
                return new LoginResultDto
                {
                    Exitoso = false,
                    Mensaje = "Credenciales inválidas"
                };
            }

            
            if (usuario.Bloqueado && usuario.FechaBloqueo.HasValue)
            {
                var tiempoTranscurrido = DateTime.UtcNow - usuario.FechaBloqueo.Value;
                if (tiempoTranscurrido.TotalMinutes < 15)
                {
                    var minutosRestantes = 15 - (int)tiempoTranscurrido.TotalMinutes;
                    return new LoginResultDto
                    {
                        Exitoso = false,
                        Mensaje = $"Cuenta bloqueada. Intente nuevamente en {minutosRestantes} minutos"
                    };
                }
                else
                {
                    
                    usuario.Bloqueado = false;
                    usuario.IntentosLogin = 0;
                    usuario.FechaBloqueo = null;
                    await _usuarioRepository.ActualizarAsync(usuario);
                }
            }

            
            if (!VerificarPassword(dto.Password, usuario.PasswordHash))
            {
                usuario.IntentosLogin++;

                if (usuario.IntentosLogin >= 5)
                {
                    usuario.Bloqueado = true;
                    usuario.FechaBloqueo = DateTime.UtcNow;
                    await _usuarioRepository.ActualizarAsync(usuario);

                    return new LoginResultDto
                    {
                        Exitoso = false,
                        Mensaje = "Cuenta bloqueada por 15 minutos debido a múltiples intentos fallidos"
                    };
                }

                await _usuarioRepository.ActualizarAsync(usuario);

                return new LoginResultDto
                {
                    Exitoso = false,
                    Mensaje = $"Credenciales inválidas. Intento {usuario.IntentosLogin} de 5"
                };
            }

            
            usuario.IntentosLogin = 0;
            await _usuarioRepository.ActualizarAsync(usuario);

            var token = _jwtService.GenerarToken(usuario);

            return new LoginResultDto
            {
                Exitoso = true,
                Token = token,
                Mensaje = "Autenticación exitosa"
            };
        }

        private bool EsEmailValido(string email)
        {
            var patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, patron);
        }

        private bool EsPasswordValido(string password)
        {
            if (password.Length < 8) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsDigit)) return false;
            if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerificarPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}

