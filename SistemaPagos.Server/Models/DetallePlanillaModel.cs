using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SistemaPagos.Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class DetallePlanillaModel
{
    [Key]
    public int IdDetalle { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal SalarioBruto { get; set; }
    public decimal Isss { get; set; }
    public decimal Afp { get; set; }
    public decimal Renta { get; set; }
    public decimal SalarioNeto { get; set; }

    [ForeignKey("IdPlanilla")]
    public PlanillaModel? Planilla { get; set; }
    public int IdPlanilla { get; set; }

    [ForeignKey("IdUsuario")]
    public UsuarioModel? Usuario { get; set; }
    public int IdUsuario { get; set; }
}
