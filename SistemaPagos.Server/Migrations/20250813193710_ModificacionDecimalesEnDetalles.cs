using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPagos.Server.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionDecimalesEnDetalles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SalarioNeto",
                table: "Detalles",
                type: "decimal(7,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalarioBruto",
                table: "Detalles",
                type: "decimal(7,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Renta",
                table: "Detalles",
                type: "decimal(7,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Isss",
                table: "Detalles",
                type: "decimal(7,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Afp",
                table: "Detalles",
                type: "decimal(7,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SalarioNeto",
                table: "Detalles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalarioBruto",
                table: "Detalles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Renta",
                table: "Detalles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Isss",
                table: "Detalles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Afp",
                table: "Detalles",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");
        }
    }
}
