using Peliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Repository.IRepository
{
    public interface IUsuarioRepository
    {
        //Metodo para traer todos los usuarios 
        ICollection<Usuario> GetUsuarios();

        //Obtener un usuario  
        Usuario GetUsuario(int id_usuario);

        //Saber si existe una pelicula por nombre 
        bool ExisteUsuario(string nombre_usuario);

        //Metodo para registrar un usuario  
        Usuario Registro(Usuario usuario, string password);

        //Metodo para hacer login
        Usuario Login(string usuario, string password);

        //Guardar cambios 
        bool Guardar();

    }
}
