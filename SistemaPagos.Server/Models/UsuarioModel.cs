namespace SistemaPagos.Server.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

  public class UsuarioModel
  {
    [Key]
    public int Id_usuario { get; set; }

    [Required, StringLength(10)]
    public string Codigo_empleado { get; set; } = string.Empty;

    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(8)]
    public string Clave { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
    public bool Es_admin { get; set; } = false;

    public ICollection<PlanillaModel>? Planillas { get; set; }
    public ICollection<DetallePlanillaModel>? Detalles { get; set; }
    public ICollection<NotificacionModel>? Notificaciones { get; set; }
}

