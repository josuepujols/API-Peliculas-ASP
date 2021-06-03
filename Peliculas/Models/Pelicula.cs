using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Peliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string RutaImagen { get; set; }

        public string Descripcion { get; set; }

        public string Duracion { get; set; }

        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public TipoClasificacion Clasificacion { get; set; }

        public DateTime FechaCreacion { get; set; }

        //Relacion con la tabla Categoria 
        public int id_categoria { get; set; }
        [ForeignKey("id_categoria")]
        public Categoria Categoria { get; set; }
    }
}
