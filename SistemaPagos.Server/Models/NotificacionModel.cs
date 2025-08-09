using SistemaPagos.Server.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NotificacionModel
{
    [Key]
    public int IdNotificacion { get; set; }

    [Required]
    public string Mensaje { get; set; }= string.Empty;
    public DateTime FechaEnvio { get; set; } = DateTime.Now;
    public bool Leida { get; set; } = false;

    [ForeignKey("IdUsuario")]
    public UsuarioModel? Usuario { get; set; }
    public int IdUsuario { get; set; }

    [ForeignKey("IdPlanilla")]
    public PlanillaModel? Planilla { get; set; }
    public int IdPlanilla { get; set; } 
}

