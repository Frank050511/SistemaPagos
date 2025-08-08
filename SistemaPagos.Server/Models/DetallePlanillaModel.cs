using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SistemaPagos.Server.Models;
using System.ComponentModel.DataAnnotations.Schema;
public class DetallePlanillaModel
{
    [Key]
    public int Id_detalle { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Salario_bruto { get; set; }
    public decimal Isss { get; set; }
    public decimal Afp { get; set; }
    public decimal Renta { get; set; }
    public decimal Salario_neto { get; set; }

    [ForeignKey("Id_planilla")]
    public PlanillaModel? Planilla { get; set; }
    public int Id_planilla { get; set; }

    [ForeignKey("Id_usuario")]
    public UsuarioModel? Usuario { get; set; }
    public int Id_usuario { get; set; }
}
