using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculaActor>()
                .HasKey(x => new { x.PeliculaId, x.ActorId});

            modelBuilder.Entity<PeliculaGenero>()
                .HasKey(x => new { x.PeliculaId, x.GeneroId});

            modelBuilder.Entity<PeliculaCine>()
                .HasKey(x => new { x.PeliculaId, x.CineId});

            base.OnModelCreating(modelBuilder); 
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Cine> Cines { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
        public DbSet<PeliculaActor> PeliculaActores { get; set; }
        public DbSet<PeliculaCine> PeliculaCines { get; set; }
        public DbSet<Rating> Ratings { get; set; }

    }
}
