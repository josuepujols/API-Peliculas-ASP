using Peliculas.Datos;
using Peliculas.Models;
using Peliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        //instanciamos el contexto
        private readonly ApplicationDbContext _context;


        //constructor 
        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //Metodos de la interfaz
        public bool ActualizarCategoria(Categoria categoria)
        {
            _context.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _context.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            _context.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string name)
        {
            bool valor = _context.Categoria.Any(c => c.Nombre.ToLower().Trim() == name.ToLower().Trim());
            return valor;
        }

        public bool ExisteCategoria(int id)
        {
            return _context.Categoria.Any(c => c.Id == id);
        }

        public Categoria GetCategoria(int id_categoria)
        {
            return _context.Categoria.FirstOrDefault(c => c.Id == id_categoria);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _context.Categoria.OrderBy(c => c.Nombre).ToList();                                                                              

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
