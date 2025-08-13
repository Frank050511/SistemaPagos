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

    [HttpGet]
    public async Task<IActionResult> GetBoletas(int anio, int mes)
    {
        int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var boletas = await _context.Detalles
            .Where(d => d.IdUsuario == idUsuario &&
                        d.Planilla.FechaCorte.Year == anio &&
                        d.Planilla.FechaCorte.Month == mes &&
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

    //[HttpGet("descargar/{id}")]
    //public async Task<IActionResult> DescargarBoleta(int id)
    //{
    //    int idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    //    var boleta = await _context.Detalles
    //        .Include(d => d.Planilla)
    //        .Include(d => d.Usuario)
    //        .FirstOrDefaultAsync(d => d.IdDetalle == id && d.IdUsuario == idUsuario);

    //    if (boleta == null)
    //        return NotFound();

    //    using (var workbook = new XLWorkbook())
    //    {
    //        var worksheet = workbook.Worksheets.Add("Boleta de Pago");

    //        // Encabezado
    //        worksheet.Cell(1, 1).Value = "Boleta de Pago";
    //        worksheet.Cell(1, 1).Style.Font.Bold = true;
    //        worksheet.Range(1, 1, 1, 2).Merge();

    //        // Datos
    //        worksheet.Cell(3, 1).Value = "Código Empleado:";
    //        worksheet.Cell(3, 2).Value = boleta.Usuario.CodigoEmpleado;

    //        worksheet.Cell(4, 1).Value = "Nombre:";
    //        worksheet.Cell(4, 2).Value = boleta.Usuario.Nombre;

    //        worksheet.Cell(5, 1).Value = "Periodo:";
    //        worksheet.Cell(5, 2).Value = boleta.Planilla.FechaCorte.ToString("MMMM yyyy");

    //        worksheet.Cell(7, 1).Value = "Salario Bruto:";
    //        worksheet.Cell(7, 2).Value = boleta.SalarioBruto;

    //        worksheet.Cell(8, 1).Value = "Descuento ISSS:";
    //        worksheet.Cell(8, 2).Value = boleta.Isss;

    //        worksheet.Cell(9, 1).Value = "Descuento AFP:";
    //        worksheet.Cell(9, 2).Value = boleta.Afp;

    //        worksheet.Cell(10, 1).Value = "Renta:";
    //        worksheet.Cell(10, 2).Value = boleta.Renta;

    //        worksheet.Cell(12, 1).Value = "Salario Neto:";
    //        worksheet.Cell(12, 2).Value = boleta.SalarioNeto;
    //        worksheet.Cell(12, 2).Style.Font.Bold = true;

    //        // Autoajustar columnas
    //        worksheet.Columns().AdjustToContents();

    //        using (var stream = new MemoryStream())
    //        {
    //            workbook.SaveAs(stream);
    //            return File(
    //                stream.ToArray(),
    //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    //                $"Boleta_{boleta.Usuario.CodigoEmpleado}_{boleta.Planilla.FechaCorte:yyyyMM}.xlsx"
    //            );
    //        }
    //    }
    //}
}