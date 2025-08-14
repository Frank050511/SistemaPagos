using Microsoft.EntityFrameworkCore;
using SistemaPagos.Server.Models;
public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    //aquí se definen las entidades de los modelos: DbSet<modelo>
        public DbSet<UsuarioModel> Usuarios { get; set; } 
       public DbSet<PlanillaModel> Planillas { get; set; } 
        public DbSet<DetallePlanillaModel> Detalles { get; set; } 
        public DbSet<NotificacionModel> Notificaciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Relacion entre Planilla y Usuario
        modelBuilder.Entity<PlanillaModel>()
            .HasOne(planilla => planilla.Usuario)// en este caso, "planilla" es el objeto que tiene la FK
            .WithMany(usuario => usuario.Planillas)
            .HasForeignKey(planilla => planilla.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict); //evita que se borren usuarios si tiene planillas

        //relacion entre Planilla y DetallePlanilla
        modelBuilder.Entity<PlanillaModel>()
            .HasMany(planilla => planilla.Detalles)// en este caso, "planilla" es el objeto que tiene la FK
            .WithOne(detalles => detalles.Planilla)
            .HasForeignKey(detalles => detalles.IdPlanilla)
            .OnDelete(DeleteBehavior.Cascade); // si se borra una planilla, se borran sus detalles

        //relacion entre DetallePlanilla y Usuario
        modelBuilder.Entity<DetallePlanillaModel>()
            .HasOne(detalles => detalles.Usuario)// en este caso, "detalles" es el objeto que tiene la FK
            .WithMany(usuario => usuario.Detalles)
            .HasForeignKey(detalles => detalles.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        //relacion entre Notificacion y Usuario
        modelBuilder.Entity<NotificacionModel>()
            .HasOne(notificaciones => notificaciones.Usuario)
            .WithMany(usuario => usuario.Notificaciones)
            .HasForeignKey(notificaciones => notificaciones.IdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        //relacion entre Notificacion y Planilla
        modelBuilder.Entity<NotificacionModel>()
            .HasOne(notificaciones => notificaciones.Planilla)
            .WithMany(planilla => planilla.Notificaciones)
            .HasForeignKey(notificaciones => notificaciones.IdPlanilla)
            .OnDelete(DeleteBehavior.Cascade);
    }


}
