namespace SistemaPagos.Server.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

  public class UsuarioModel
  {
    // todas esas son las propiedades que tiene cada usuario, o diciendolo de otra forma, son los campos
    // de la tabla Usuarios en la base de datos
    [Key]
    public int IdUsuario { get; set; }

    [Required, StringLength(10)]
    public string CodigoEmpleado { get; set; } = string.Empty;

    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(60)]
    public string Clave { get; set; } = string.Empty;

    [StringLength(100)]
    [EmailAddress]
    public string? Correo { get; set; }

    public bool Activo { get; set; } = true;
    public bool EsAdmin { get; set; } = false;

    public ICollection<PlanillaModel>? Planillas { get; set; }
    public ICollection<DetallePlanillaModel>? Detalles { get; set; }
    
}

