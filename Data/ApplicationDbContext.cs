using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BarberiaLosHermanos.Models;

namespace BarberiaLosHermanos.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppointmentServicio>()
                .HasKey(a => new { a.AppointmentId, a.ProductoId });

            modelBuilder.Entity<AppointmentServicio>()
                .HasOne(a => a.Appointment)
                .WithMany(a => a.Servicios)
                .HasForeignKey(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppointmentServicio>()
                .HasOne(a => a.Producto)
                .WithMany()
                .HasForeignKey(a => a.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Cliente { get; set; } = default!;
        public DbSet<Producto> Producto { get; set; } = default!;
        public DbSet<Factura> Factura { get; set; } = default!;
        public DbSet<ProductosFactura> ProductosFactura { get; set; } = default!;
        public DbSet<Appointment> Appointment { get; set; } = default!;
        public DbSet<Barbero> Barbero { get; set; } = default!;
        public DbSet<AppointmentServicio> AppointmentServicio { get; set; } = default!;
    }
}
