using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Models.DTOS
{
    public class UsuarioAuthDto
    {
        [Key]
        public int Id { get; set; }

        //Usuario de acceso 
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string Usuario { get; set; }

        //Campo para el password 
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "La contraseña debe estar entre 4 y 10 caracteres.")]
        public string Password { get; set; }

    }
}
