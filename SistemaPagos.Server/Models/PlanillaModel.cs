using SistemaPagos.Server.Models;
using System.Collections.Generic;   
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

public class PlanillaModel
 {
    [Key]
    public int Id_planilla { get; set; }

    [StringLength(30)]
    public string Nombre_planilla { get; set; } = string.Empty;

    public DateTime Fecha_carga { get; set; } = DateTime.Now;
    public string Ruta_archivo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime Fecha_corte { get; set; }

    [ForeignKey("Id_usuario")]
    public UsuarioModel? Usuario { get; set; } 
    public int Id_usuario { get; set; }

   public ICollection<DetallePlanillaModel>? Detalles { get; set; }
}

