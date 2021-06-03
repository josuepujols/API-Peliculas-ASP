using Peliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Repository.IRepository
{
    public interface ICategoriaRepository
    {
        //Primer metodo para traer todas las categorias 
        ICollection<Categoria> GetCategorias();
        //Segundo para traer una categoria 
        Categoria GetCategoria(int id_categoria);
        //Validar si existe una categoria 
        bool ExisteCategoria(string name);
        //Validar por id
        bool ExisteCategoria(int id);
        //Crear una nueva categoria 
        bool CrearCategoria(Categoria categoria);
        //Actualizar una categoria 
        bool ActualizarCategoria(Categoria categoria);
        //Borrar Categorria 
        bool BorrarCategoria(Categoria categoria);
        //Guardar categoria
        bool Guardar();
    }
}
