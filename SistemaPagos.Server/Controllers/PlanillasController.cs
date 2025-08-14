using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using SistemaPagos.Server.Services;

[Authorize(Roles = "admin")]
[Route("api/[controller]")]
[ApiController]
public class PlanillasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailService _emailService;

    public PlanillasController(ApplicationDbContext context, IWebHostEnvironment env, IEmailService emailService)
    {
        _context = context;
        _env = env;
        _emailService = emailService;
    }

    // este endpoint es el que se encarga de generar la plantilla utilizando ClosedXML y descargarla
    [HttpGet("plantilla")]
    public IActionResult DescargarPlantilla()
    {
        try
        {
            using (var memoryStream = new MemoryStream())
            {
                // aqui se crea el archivo de excel en el que se crea la estructura de la plantilla
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("PlantillaBoletas"); //crea la hoja de calculo

                    // Estos son los encabezados de la plantilla
                    var headers = new string[] {
                        "CodigoEmpleado", "NombreEmpleado", "FechaCorte",
                        "SalarioBruto", "ISSS", "AFP", "Renta", "SalarioNeto"
                    };

                    // Configuracion del formato de los encabezados
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];    //nombre del encabezado
                        cell.Style.Font.Bold = true; //negrita
                        cell.Style.Fill.BackgroundColor = XLColor.LightGray; //color de fondo
                    }

                    // Datos de ejemplo. Se deben eliminar al usar la plantilla
                    worksheet.Cell(2, 1).Value = "EMP001";
                    worksheet.Cell(2, 2).Value = "Juan Pérez";
                    worksheet.Cell(2, 3).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    worksheet.Cell(2, 4).Value = 1000.00;
                    worksheet.Cell(2, 5).Value = 30.00;
                    worksheet.Cell(2, 6).Value = 72.50;
                    worksheet.Cell(2, 7).Value = 100.00;
                    worksheet.Cell(2, 8).Value = 797.50;

                    // Formato de números. Aqui se define el formato de moneda para las columnas correspondientes
                    worksheet.Column(4).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(5).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(6).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(7).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(8).Style.NumberFormat.Format = "$#,##0.00";

                    // Autoajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Guarda el archivo en el MemoryStream
                    workbook.SaveAs(memoryStream);
                }

                memoryStream.Position = 0; // Resetear posición para leer desde el inicio

                // Envia el archivo para que pueda ser descargado
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

    // Este endpoint permite cargar o subir un archivo al sistema. El archivo sería la planilla de pagos
    [HttpPost("cargar")]
    public async Task<IActionResult> CargarPlanilla([FromForm] PlanillaCargaDto dto)
    {
        if (dto.Archivo == null || dto.Archivo.Length == 0)
            return BadRequest("Archivo no válido"); //indica que el archivo no es valido si no contiene datos

        var detalles = await _ProcesarExcel(dto.Archivo);
        if (detalles.Count == 0)
            return BadRequest("El Excel no contiene datos válidos");

        string rutaArchivo = _GuardarArchivo(dto.Archivo); // Guarda el archivo en el servidor

        //Llena los campos de la base de datos para registrar la planilla.
        var planilla = new PlanillaModel
        {
            NombrePlanilla = Path.GetFileNameWithoutExtension(dto.Archivo.FileName), //obtiene el nombre del archivo sin la extension
            FechaCarga = DateTime.Now, //fecha en la que se carga la planilla. obtiene la fecha automáticamente
            RutaArchivo = rutaArchivo, //guarda la ruta del archivo en el servidor
            Activo = true,             //Si es true significa que la planilla se puede ver, si es false significa que está "eliminada"
            FechaCorte = dto.FechaCorte,//Fecha de corte de la planilla, que se obtiene del formulario
            IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Detalles = detalles
        };

        _context.Planillas.Add(planilla);
        await _context.SaveChangesAsync();

        // Obtener correos de los usuarios afectados
        var correos = await _context.Detalles
            .Where(d => d.Planilla.IdPlanilla == planilla.IdPlanilla)
            .Select(d => d.Usuario.Correo)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .ToListAsync();

        // Enviar correos (en segundo plano)
        foreach (var correo in correos)
        {
            _ = _emailService.EnviarCorreoAsync(
                correo!,
                "Nueva boleta de pago disponible",
                $"<p>Se ha cargado una nueva boleta para el período {planilla.FechaCorte:MMMM yyyy}</p>"
            );
        }

        return Ok(new { id = planilla.IdPlanilla });
    }
    // GET: api/planillas manda a llamar a las planillas
    [HttpGet]
    public async Task<IActionResult> GetPlanillas()
    {
        //Crea la consulta para obtener las planillas activas
        var planillas = await _context.Planillas
            .Where(p => p.Activo)
            .Select(p => new {
                p.IdPlanilla,
                p.NombrePlanilla,
                p.FechaCorte,
                p.RutaArchivo,
                FechaCarga = p.FechaCarga.ToString("yyyy-MM-dd")
            })
            .ToListAsync();

        return Ok(planillas);
    }

    // GET: api/planillas/descargar Se usa para descargar una de las planillas que se han subido previamente
    [HttpGet("descargar")]
    public IActionResult DescargarPlanilla([FromQuery] string ruta)
    {
        if (!System.IO.File.Exists(ruta))
            return NotFound("Archivo no encontrado");

        var fileStream = System.IO.File.OpenRead(ruta);
        return File(fileStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Path.GetFileName(ruta));
    }

    // DELETE: api/planillas/{id} elimina la planilla de la vista por medio del id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlanilla(int id)
    {
        var planilla = await _context.Planillas.FindAsync(id);
        if (planilla == null)
            return NotFound();

        // Marcamos como inactivo en lugar de borrar físicamente
        planilla.Activo = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Este método privado procesa el archivo Excel y extrae los detalles de la planilla para guardarlos en la base de datos
    private async Task<List<DetallePlanillaModel>> _ProcesarExcel(IFormFile archivo)
    {
        var detalles = new List<DetallePlanillaModel>(); // Lista para almacenar los detalles de la planilla en la tabla Detalles

        using (var stream = new MemoryStream())
        {
            await archivo.CopyToAsync(stream);

            using (var workbook = new XLWorkbook(stream)) // Abre el archivo Excel de la planilla
            {
                var worksheet = workbook.Worksheet(1); // Primera hoja
                var rows = worksheet.RowsUsed().Skip(1); // Saltar encabezados

                // Recorre cada fila del archivo Excel para extraer los datos
                foreach (var row in rows)
                {
                    string codigoEmpleado = row.Cell(1).GetString();

                    var empleado = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.CodigoEmpleado == codigoEmpleado && u.Activo);

                    if (empleado == null) continue;

                    detalles.Add(new DetallePlanillaModel
                    {
                        //ingresa los datos de las celdas en la tabla Detalles
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

    // Este método privado guarda el archivo en una carpeta del servidor y retorna la ruta donde se guardó
    private string _GuardarArchivo(IFormFile archivo)
    {
        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "planillas"); // Carpeta donde se guardan los archivos
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