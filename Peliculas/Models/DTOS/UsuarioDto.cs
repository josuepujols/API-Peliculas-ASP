using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Models.DTOS
{
    public class UsuarioDto
    {
        //Usuario de acceso 
        public string UsuarioA { get; set; }

        //Password encriptada 
        public byte[] PasswordHash { get; set; }
    }
}
