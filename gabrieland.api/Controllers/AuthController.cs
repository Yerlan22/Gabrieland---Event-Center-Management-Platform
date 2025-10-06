// Controllers/AuthController.cs
using gabrieland.api.Models;
using gabrieland.Data;
using gabrieland.api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataBaseConnector _db;
    private readonly EmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(DataBaseConnector db, IConfiguration configuration,
                     ILogger<AuthController> logger, EmailService emailService)
    {
        _db = db;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var hashedPassword = Encrytion.ComputeSha256Hash(loginDto.Contraseña);
            var usuario = await _db.AuthenticateUsuario(loginDto.UserName, hashedPassword);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Credenciales inválidas"
                });
            }

            // Generate JWT token
            var token = GenerateJwtToken(usuario);

            return Ok(new
            {
                Success = true,
                Token = token,
                ExpiresIn = _configuration.GetValue<int>("Jwt:ExpiryInMinutes") * 60,
                User = new
                {
                    usuario.ID_Usuario,
                    usuario.nombre,
                    usuario.apellido,
                    usuario.correo,
                    usuario.TUO_ID_Tipo_Usuario
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Error interno del servidor"
            });
        }
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Usuario user)
    {        
        try
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            if (await _db.UsuarioExists(user.UserName))
            {
                return Conflict(new
                {
                    Success = false,
                    Message = "Usuario ya existe"
                });
            }
            if (await _db.CorreoExists(user.correo))
            {
                return Conflict(new
                {
                    Success = false,
                    Message = "El correo ingresado ya existe"
                });
            }

            // Create user (your existing database function)
            var userId = await _db.CreateUsuario(user);
            try
            {
                await _emailService.SendConfirmationEmail(user.correo, user.UserName);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo de confirmación.");
            }

            
            // Return success response
            return Ok(new
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                UserId = userId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante el registro");
            return StatusCode(500, new
            {
                Success = false,
                Message = "Ocurrió un error al registrar el usuario"
            });
        }
    }
    private string GenerateJwtToken(Usuario usuario)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        string rol = usuario.TUO_ID_Tipo_Usuario switch
        {
            2 => "Admin",
            1 => "Cliente",
            3 => "SUDO"
        };


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.ID_Usuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.correo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            new Claim(ClaimTypes.Name, usuario.nombre),
            new Claim(ClaimTypes.Surname, usuario.apellido),
            new Claim(ClaimTypes.Email, usuario.correo),
            new Claim(ClaimTypes.Role, rol),
            new Claim("role", rol)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiryInMinutes")),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}