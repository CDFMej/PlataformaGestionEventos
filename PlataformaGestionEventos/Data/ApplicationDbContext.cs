using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlataformaGestionEventos.Models;

namespace PlataformaGestionEventos.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Sala> Salas { get; set; }
    /*
    public DbSet<Evento> Eventos { get; set; }
    public DbSet<Asistente> Asistentes { get; set; }
    public DbSet<Inscripcion> Inscripciones { get; set; }
    public DbSet<Recurso> Recursos { get; set; }
    public DbSet<RecursoEvento> RecursoEvento { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RecursoEvento>()
            .HasKey(re => new { re.EventoId, re.RecursoId });
    }
    */
}