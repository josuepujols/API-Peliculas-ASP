using Microsoft.EntityFrameworkCore;
using Peliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   

namespace Peliculas.Datos
{ 
    public class ApplicationDbContext : DbContext 
    {
        //Creamos nuestro constructor 
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options ):base(options)
        {

        }

        //Crear el DbSet para categoria 
        public DbSet<Categoria> Categoria { get; set; }

        //Crear el DbSet para Pelicula 
        public DbSet<Pelicula> Pelicula { get; set; }

        //Crear el DbSet para Usuario  
        public DbSet<Usuario> Usuario { get; set; }
    }
}
