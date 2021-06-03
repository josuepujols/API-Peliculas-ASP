using Microsoft.EntityFrameworkCore;
using Peliculas.Datos;
using Peliculas.Models;
using Peliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Repository
{
    public class PeliculaRepository : IPeliculaRepository
    {
        //instanciamos el contexto
        private readonly ApplicationDbContext _context;


        //constructor 
        public PeliculaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            _context.Pelicula.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _context.Pelicula.Remove(pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _context.Pelicula;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }

            //Retornamos la busqueda 
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            _context.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(string nombre_pelicula)
        {
            bool valor = _context.Pelicula.Any(c => c.Nombre.ToLower().Trim() == nombre_pelicula.ToLower().Trim());
            return valor;
        }

        public bool ExistePelicula(int id_pelicula)
        {
            return _context.Pelicula.Any(c => c.Id == id_pelicula);
        }

        public Pelicula GetPelicula(int id_pelicula)
        {
            return _context.Pelicula.FirstOrDefault(c => c.Id == id_pelicula);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _context.Pelicula.OrderBy(c => c.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int CatId)
        {
            return _context.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.id_categoria == CatId).ToList();
        }

        public bool Guardar()
        {
            if (_context.SaveChanges() >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
