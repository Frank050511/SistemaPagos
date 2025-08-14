using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BoletasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BoletasController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Este endpoint obtiene la informacion de las boletas de pago del usuario logueado autenticado para un año y mes específicos.
    [HttpGet]
    public async Task<IActionResult> GetBoletas(int anio, int mes)
    {
        int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var boletas = await _context.Detalles
            .Where(d => d.IdUsuario == idUsuario &&
                        d.Planilla.FechaCorte.Year == anio && //aqui se filtran los datos
                        d.Planilla.FechaCorte.Month == mes && //por año y mes
                        d.Planilla.Activo)
            .Include(d => d.Planilla)
            .Include(d => d.Usuario)
            .Select(d => new
            {
                CodigoEmpleado = d.Usuario.CodigoEmpleado,
                NombreEmpleado = d.Usuario.Nombre,
                Corte = d.Planilla.FechaCorte,
                d.SalarioBruto,
                d.Isss,
                d.Afp,
                d.Renta,
                d.SalarioNeto
            }).ToListAsync();

        return Ok(boletas);
    }
}