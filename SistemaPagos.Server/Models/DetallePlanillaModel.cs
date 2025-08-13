using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SistemaPagos.Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class DetallePlanillaModel
{
    [Key]
    public int IdDetalle { get; set; }

    [Range(0.01, double.MaxValue)]

    [Column(TypeName = "decimal(7,2)")]
    public decimal SalarioBruto { get; set; }

    [Column(TypeName = "decimal(7,2)")]
    public decimal Isss { get; set; }

    [Column(TypeName = "decimal(7,2)")]
    public decimal Afp { get; set; }

    [Column(TypeName = "decimal(7,2)")]
    public decimal Renta { get; set; }

    [Column(TypeName = "decimal(7,2)")]
    public decimal SalarioNeto { get; set; }

    [ForeignKey("IdPlanilla"), Required]
    public PlanillaModel Planilla { get; set; } = null!;
    public int IdPlanilla { get; set; }

    [ForeignKey("IdUsuario"), Required]
    public UsuarioModel Usuario { get; set; } = null!;
    public int IdUsuario { get; set; }
}
