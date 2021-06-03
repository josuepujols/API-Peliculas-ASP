using Peliculas.Datos;
using Peliculas.Models;
using Peliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        //instanciamos el contexto
        private readonly ApplicationDbContext _context;


        //constructor 
        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ExisteUsuario(string nombre_usuario)
        {
            if (_context.Usuario.Any(x => x.UsuarioA == nombre_usuario))
            {
                return true;
            }
            return false;

        }

        public Usuario GetUsuario(int id_usuario)
        {
            return _context.Usuario.FirstOrDefault(c => c.Id == id_usuario);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _context.Usuario.OrderBy(c => c.UsuarioA).ToList();
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

        public Usuario Login(string usuario, string password)
        {
            var user = _context.Usuario.FirstOrDefault(x => x.UsuarioA == usuario);
            
            if (user == null)
            {
                return null;
            }

            if (!ValidarPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            //Retornamos el usuario si todo es correcto 
            return user;
        }

        public Usuario Registro(Usuario usuario, string password)
        {
            byte[] passwordHash, passwordSalt;

            CrearPasswordHash(password, out passwordHash, out passwordSalt);

            //Asignamos las propiedades
            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            _context.Usuario.Add(usuario);
            Guardar();
            return usuario;

        }


        //Metodo para validar el password
        private bool ValidarPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hasComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < hasComputado.Length; i++)
                {
                    if (hasComputado[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        //Metodo registrar un nuevo usuario 
        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}
