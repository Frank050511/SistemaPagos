using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize(Roles = "admin")]
[Route("api/[controller]")]
[ApiController]
public class PlanillasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PlanillasController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/planillas/plantilla
    [HttpGet("plantilla")]
    public IActionResult DescargarPlantilla()
    {
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                // Crear libro de trabajo
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("PlantillaBoletas");

                    // Encabezados
                    var headers = new string[] {
                        "CodigoEmpleado", "NombreEmpleado", "FechaCorte",
                        "SalarioBruto", "ISSS", "AFP", "Renta", "SalarioNeto"
                    };

                    // Formato de encabezados
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    }

                    // Datos de ejemplo (fila 2)
                    worksheet.Cell(2, 1).Value = "EMP001";
                    worksheet.Cell(2, 2).Value = "Juan Pérez";
                    worksheet.Cell(2, 3).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    worksheet.Cell(2, 4).Value = 1000.00;
                    worksheet.Cell(2, 5).Value = 30.00;
                    worksheet.Cell(2, 6).Value = 72.50;
                    worksheet.Cell(2, 7).Value = 100.00;
                    worksheet.Cell(2, 8).Value = 797.50;

                    // Formato de números
                    worksheet.Column(4).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(5).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(6).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(7).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(8).Style.NumberFormat.Format = "$#,##0.00";

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Guardar en el MemoryStream
                    workbook.SaveAs(memoryStream);
                }

                memoryStream.Position = 0; // Resetear posición

                // Devolver archivo
                return File(
                    memoryStream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Plantilla_Boletas.xlsx"
                );
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al generar plantilla: {ex.Message}");
        }
    }

    // POST: api/planillas/cargar
    [HttpPost("cargar")]
    public async Task<IActionResult> CargarPlanilla([FromForm] PlanillaCargaDto dto)
    {
        if (dto.Archivo == null || dto.Archivo.Length == 0)
            return BadRequest("Archivo no válido");

        var detalles = await _ProcesarExcel(dto.Archivo);
        if (detalles.Count == 0)
            return BadRequest("El Excel no contiene datos válidos");

        string rutaArchivo = _GuardarArchivo(dto.Archivo);

        var planilla = new PlanillaModel
        {
            NombrePlanilla = Path.GetFileNameWithoutExtension(dto.Archivo.FileName),
            FechaCarga = DateTime.Now,
            RutaArchivo = rutaArchivo,
            Activo = true,
            FechaCorte = dto.FechaCorte,
            IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Detalles = detalles
        };

        _context.Planillas.Add(planilla);
        await _context.SaveChangesAsync();

        return Ok(new { id = planilla.IdPlanilla });
    }

    private async Task<List<DetallePlanillaModel>> _ProcesarExcel(IFormFile archivo)
    {
        var detalles = new List<DetallePlanillaModel>();

        using (var stream = new MemoryStream())
        {
            await archivo.CopyToAsync(stream);

            using (var workbook = new XLWorkbook(stream))
            {
                var worksheet = workbook.Worksheet(1); // Primera hoja
                var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                foreach (var row in rows)
                {
                    string codigoEmpleado = row.Cell(1).GetString();

                    var empleado = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.CodigoEmpleado == codigoEmpleado && u.Activo);

                    if (empleado == null) continue;

                    detalles.Add(new DetallePlanillaModel
                    {
                        IdUsuario = empleado.IdUsuario,
                        SalarioBruto = row.Cell(4).GetValue<decimal>(),
                        Isss = row.Cell(5).GetValue<decimal>(),
                        Afp = row.Cell(6).GetValue<decimal>(),
                        Renta = row.Cell(7).GetValue<decimal>(),
                        SalarioNeto = row.Cell(8).GetValue<decimal>()
                    });
                }
            }
        }

        return detalles;
    }

    private string _GuardarArchivo(IFormFile archivo)
    {
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "planillas");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = $"{Guid.NewGuid()}_{archivo.FileName}";
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            archivo.CopyTo(fileStream);
        }

        return filePath;
    }
}
public class PlanillaCargaDto
{
    public required DateTime FechaCorte { get; set; }
    public required IFormFile Archivo { get; set; }
}