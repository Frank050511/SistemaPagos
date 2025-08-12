using Microsoft.AspNetCore.Mvc;
using SistemaPagos.Server.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto registrarDto)
    {
        // Validar si el código de empleado ya existe
        if (await _context.Usuarios.AnyAsync(u => u.CodigoEmpleado == registrarDto.CodigoEmpleado))
        {
            return BadRequest("El código de empleado ya está registrado");
        }

        // Validar el modelo
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Crear nuevo usuario
        var usuario = new UsuarioModel
        {
            CodigoEmpleado = registrarDto.CodigoEmpleado,
            Nombre = registrarDto.Nombre,
            Clave = BCrypt.Net.BCrypt.HashPassword(registrarDto.Clave),
            Activo = true,
            EsAdmin = registrarDto.EsAdmin
        };

        // Guardar en la base de datos
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Generar token JWT
        var token = GenerateJwtToken(usuario);

        // Retornar respuesta
        return Ok(new
        {
            token,
            usuario = new
            {
                usuario.IdUsuario,
                usuario.CodigoEmpleado,
                usuario.Nombre,
                usuario.EsAdmin
            }
        });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        // 1. Validar usuario
        var usuario = _context.Usuarios.FirstOrDefault(u => u.CodigoEmpleado == loginDto.CodigoEmpleado);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Clave, usuario.Clave))
            return Unauthorized("Credenciales invalidas");

        Console.WriteLine($"Usuario {usuario.CodigoEmpleado} - EsAdmin: {usuario.EsAdmin}"); // Log para depuración


        // 2. Generar JWT
        var token = GenerateJwtToken(usuario);

        // 3. Retornar respuesta
        return Ok(new
        {
            token,
            usuario = new
            {
                usuario.IdUsuario,
                usuario.CodigoEmpleado,
                usuario.EsAdmin,
                usuario.Nombre
            }
        });
    }

    private string GenerateJwtToken(UsuarioModel usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Role, usuario.EsAdmin ? "admin" : "user")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginDto
{
    public required string CodigoEmpleado { get; set; }
    public required string Clave { get; set; }
}

public class RegistrarUsuarioDto
{
    [Required, StringLength(10)]
    public required string CodigoEmpleado { get; set; }

    [Required, StringLength(150)]
    public required string Nombre { get; set; }

    [Required, StringLength(60)]
    public required string Clave { get; set; }

    public bool EsAdmin { get; set; } = false;
}