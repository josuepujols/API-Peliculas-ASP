using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Peliculas.Models.Pelicula;

namespace Peliculas.Models.DTOS
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string RutaImagen { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Duracion { get; set; }

        public TipoClasificacion Clasificacion { get; set; }


        //Relacion con la tabla Categoria 
        public int id_categoria { get; set; }
        public Categoria Categoria { get; set; }
    }
}
