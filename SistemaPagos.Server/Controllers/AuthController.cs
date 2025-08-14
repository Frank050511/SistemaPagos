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

    //este endpoint registra un nuevo usuario en el sistema. Esto está pensado para una actualizacion en el futuro,
    //ya que el registro de usuarios no debería ser administrado por un administrador de nominas.
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto registrarDto)
    {
        Console.WriteLine($"Correo recibido: {registrarDto.Correo}");
        // Valida si el código de empleado ya existe
        if (await _context.Usuarios.AnyAsync(u => u.CodigoEmpleado == registrarDto.CodigoEmpleado))
        {
            return BadRequest("El código de empleado ya está registrado");
        }

        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Crea un nuevo usuario para el sistema
        var usuario = new UsuarioModel
        {
            CodigoEmpleado = registrarDto.CodigoEmpleado,
            Nombre = registrarDto.Nombre,
            Clave = BCrypt.Net.BCrypt.HashPassword(registrarDto.Clave), // hasheamos la clave para que no sea visible en la base de datos
            Activo = true,
            EsAdmin = registrarDto.EsAdmin,
            Correo=registrarDto.Correo?.Trim()
        };

        // Guardamos en la base de datos los datos del usuario creado
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Genera un token JWT para el usuario recién registrado
        var token = GenerateJwtToken(usuario);


        Console.WriteLine($"Correo recibido: {registrarDto.Correo}");
        Console.WriteLine($"Correo a guardar: {usuario.Correo}");
        // Retornamos la respuesta para confirmar el registro y el token exitoso
        return Ok(new
        {
            token,
            usuario = new
            {
                usuario.IdUsuario,
                usuario.CodigoEmpleado,
                usuario.Nombre,
                usuario.EsAdmin,
                usuario.Correo
            }

        });
    }

    // este endpoint permite a un usuario loguearse en el sistema y obtener un token JWT
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        // Valida el usuario para confirmar que la clave y el código de empleado ingresados son correctos
        var usuario = _context.Usuarios.FirstOrDefault(u => u.CodigoEmpleado == loginDto.CodigoEmpleado);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Clave, usuario.Clave))
            return Unauthorized("Credenciales invalidas");

        Console.WriteLine($"Usuario {usuario.CodigoEmpleado} - EsAdmin: {usuario.EsAdmin}"); // Log para depuración


        // Generar JWT para que pueda acceder a la pagina siguiente
        var token = GenerateJwtToken(usuario);

        
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

    //aquí se genera el token JWT para el usuario cuando inicia sesion o se registra
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

    [StringLength(100)]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;
}