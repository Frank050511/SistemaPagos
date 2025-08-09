using SistemaPagos.Server.Models;
using System.Collections.Generic;   
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

public class PlanillaModel
 {
    [Key]
    public int IdPlanilla { get; set; }

    [StringLength(30)]
    public string NombrePlanilla { get; set; } = string.Empty;

    public DateTime FechaCarga { get; set; } = DateTime.Now;
    public string RutaArchivo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCorte { get; set; }

    [ForeignKey("IdUsuario")]
    public UsuarioModel? Usuario { get; set; } 
    public int IdUsuario { get; set; }

   public ICollection<DetallePlanillaModel>? Detalles { get; set; }
    public ICollection<NotificacionModel>? Notificaciones { get; set; }
}

