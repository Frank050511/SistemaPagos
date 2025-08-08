using Microsoft.EntityFrameworkCore;
using SistemaPagos.Server.Models;
public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    //aquí se definen las entidades del modelo: DbSet<modelo>
        public DbSet<SistemaPagos.Server.Models.UsuarioModel> Usuarios { get; set; } 
}
