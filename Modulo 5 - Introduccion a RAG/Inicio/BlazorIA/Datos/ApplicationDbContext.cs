using BlazorIA.Entidades;
using Microsoft.EntityFrameworkCore;

namespace BlazorIA.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Persona>().HasData(
            new Persona
            {
                Id = 1,
                Nombre = "Felipe Gavilán",
                Email = "felipe.gavilan@example.com",
                Salario = 45000m,
                Activo = true
            },
            new Persona
            {
                Id = 2,
                Nombre = "María López",
                Email = "maria.lopez@example.com",
                Salario = 52000m,
                Activo = true
            },
            new Persona
            {
                Id = 3,
                Nombre = "Carlos Rodríguez",
                Email = "carlos.rodriguez@example.com",
                Salario = 61000m,
                Activo = false
            },
            new Persona
            {
                Id = 4,
                Nombre = "Ana Martínez",
                Email = "ana.martinez@example.com",
                Salario = 48000m,
                Activo = false
            },
            new Persona
            {
                Id = 5,
                Nombre = "Luis Gómez",
                Email = "luis.gomez@example.com",
                Salario = 55000m,
                Activo = true
            }
        );

        }

        public DbSet<Persona> Personas { get; set; }
    }
}
