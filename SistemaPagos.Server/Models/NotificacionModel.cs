using SistemaPagos.Server.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class NotificacionModel
{
    [Key]
    public int Id_notificacion { get; set; }

    [Required]
    public string? Mensaje { get; set; }
    public DateTime Fecha_envio { get; set; } = DateTime.Now;
    public bool Leida { get; set; } = false;

    [ForeignKey("Id_usuario")]
    public UsuarioModel? Usuario { get; set; }
    public int Id_usuario { get; set; }

    [ForeignKey("Id_planilla")]
    public PlanillaModel? Planilla { get; set; }
    public int Id_planilla { get; set; } 
}

