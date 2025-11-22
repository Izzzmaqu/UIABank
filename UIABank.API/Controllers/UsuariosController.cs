using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UIABank.BW.Interfaces.BW;

namespace UIABank.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registro")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegistroUsuarioDto dto)
        {
            try
            {
                var usuario = await _usuarioService.RegistrarUsuarioAsync(dto);
                return Ok(new
                {
                    mensaje = "Usuario registrado exitosamente",
                    usuarioId = usuario.Id,
                    email = usuario.Email,
                    rol = usuario.Rol
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var resultado = await _usuarioService.AutenticarAsync(dto);

                if (!resultado.Exitoso)
                {
                    return Unauthorized(new { mensaje = resultado.Mensaje });
                }

                return Ok(new
                {
                    token = resultado.Token,
                    mensaje = resultado.Mensaje
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor" });
            }
        }
    }
}

