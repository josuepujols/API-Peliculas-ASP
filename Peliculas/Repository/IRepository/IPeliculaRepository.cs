using Peliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Repository.IRepository
{
    public interface IPeliculaRepository
    {
        //Metodo para traer todas las peliculas 
        ICollection<Pelicula> GetPeliculas();

        //Metodo para traer pelicualas por categoria 
        ICollection<Pelicula> GetPeliculasEnCategoria(int id_categoria);

        //Obtener una pelicula 
        Pelicula GetPelicula(int id_pelicula);

        //Saber si existe una pelicula por nombre 
        bool ExistePelicula(string nombre_pelicula);

        //Saber si existe una pelicula por id
        bool ExistePelicula(int id_pelicula);

        //Filtrar las peliculas 
        IEnumerable<Pelicula> BuscarPelicula(string nombre);

        //Agregar una pelicula 
        bool CrearPelicula(Pelicula pelicula);

        //Actualizar una pelicula 
        bool ActualizarPelicula(Pelicula pelicula);

        //Borrar una pelicula 
        bool BorrarPelicula(Pelicula pelicula);

        //Guardar cambios 
        bool Guardar();

    }
}
