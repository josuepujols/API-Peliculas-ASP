using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Models
{
    public class Usuario
    {
        //Propiedades para el modelo 
        //Id del usuario 
        [Key]
        public int Id { get; set; }

        //Usuario de acceso 
        public string UsuarioA { get; set; }

        //Password encriptada 
        public byte[] PasswordHash { get; set; }

        //Password con otra encriptacion
        public byte[] PasswordSalt { get; set; }

    }
}
