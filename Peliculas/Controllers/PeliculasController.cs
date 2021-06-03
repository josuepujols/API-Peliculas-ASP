using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models;
using Peliculas.Models.DTOS;
using Peliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Peliculas.Controllers
{
    [Route("api/Peliculas")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculas")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PeliculasController : Controller
    {
        private readonly IPeliculaRepository _pelRepo;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        //Constructor 
        public PeliculasController(IPeliculaRepository pelRepo, IMapper mapper, IWebHostEnvironment hosting)
        {
            _pelRepo = pelRepo;
            _mapper = mapper;
            _hostingEnvironment = hosting;
        }

        //Metodo para obtener todas las peliculas
        /// <summary>
        /// Obtener todas las peliculas 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PeliculaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetPeliculas()
        {
            var lista_peliculas = _pelRepo.GetPeliculas();

            var lista_dto = new List<PeliculaDto>();
            foreach (var lista in lista_peliculas)
            {
                lista_dto.Add(_mapper.Map<PeliculaDto>(lista));
            }

            return Ok(lista_dto);
        }


        //Metodo para traer una pelicula por el id
        /// <summary>
        /// Obtener una pelicula en especifico 
        /// </summary>
        /// <param name="id_pelicula">ID de la pelicula a buscar</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id_pelicula:int}", Name = "GetPelicula")]
        [ProducesResponseType(200, Type = typeof(PeliculaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPelicula(int id_pelicula)
        {
            var item_pelicula = _pelRepo.GetPelicula(id_pelicula);

            //Comprobamos si la pelicula existe 
            if (item_pelicula == null)
            {
                return NotFound();
            }
            else
            {
                var item_pelicula_dto = _mapper.Map<PeliculaDto>(item_pelicula);
                return Ok(item_pelicula_dto);
            }

        }

        //Metodo para buscar por categoria 
        /// <summary>
        /// Buscar por categoria
        /// </summary>
        /// <param name="id_categoria">ID de la categoria para traer todas las peliculas de esta</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{id_categoria:int}")]
        [ProducesResponseType(200, Type = typeof(PeliculaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPeliculasEnCategoria(int id_categoria)
        {
            var lista_pelicula = _pelRepo.GetPeliculasEnCategoria(id_categoria);

            if (lista_pelicula == null)
            {
                return NotFound();
            }

            var item_pelicula = new List<PeliculaDto>();

            foreach (var item in lista_pelicula)
            {
                item_pelicula.Add(_mapper.Map<PeliculaDto>(item));
            }

            return Ok(item_pelicula);
        }


        //Metodo para buscar una peluicula por el nombre 
        /// <summary>
        /// Buscar peliculas por nombre relacionado 
        /// </summary>
        /// <param name="nombre">Nombre o palabras claves para buscar</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Buscar")]
        [ProducesResponseType(200, Type = typeof(PeliculaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = _pelRepo.BuscarPelicula(nombre);

                //Comprobamos si la consulta trajo algo 
                if (resultado.Any())
                {
                    return Ok(resultado);
                }

                //Si la consulta no trajo nada retorno un not found 
                return NotFound();

            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al recuperar datos de la base de datos.");
            }
        }


        //Metodo para crear una pelicula 
        /// <summary>
        /// Agregar una nueva pelicula 
        /// </summary>
        /// <param name="pelicula_dto">Objeto con todos los campos a agregar</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaCreateDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearPelicula([FromForm] PeliculaCreateDto pelicula_dto)
        {
            //Comprobamos si el objeto esta vacio 
            if (pelicula_dto == null)
            {
                return BadRequest(ModelState);
            }

            //Comprobamos si la pelicula existe 
            if (_pelRepo.ExistePelicula(pelicula_dto.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe.");
                return StatusCode(404, ModelState);
            }


            //Implementamos la subida de archivos 
            var archivo = pelicula_dto.Foto;
            string ruta_principal = _hostingEnvironment.WebRootPath;
            var archivos = HttpContext.Request.Form.Files;

            if (archivo.Length > 0)
            {
                //Nueva imagen 
                var nombre_foto = Guid.NewGuid().ToString();
                var subidas = Path.Combine(ruta_principal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(subidas, nombre_foto + extension), FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }
                pelicula_dto.RutaImagen = @"\fotos\" + nombre_foto + extension;
            }


            var new_pelicula = _mapper.Map<Pelicula>(pelicula_dto);

            //Comprobamos si se pudo crear la pelicula con exito 
            if (!_pelRepo.CrearPelicula(new_pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal al insertar la pelicula {new_pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            //Si todo sale bien insertamos los datos 
            return CreatedAtRoute("GetPelicula", new { id_pelicula = new_pelicula.Id }, new_pelicula);

        }

        //Metodo para actualizar una pelicula 
        /// <summary>
        /// Actualizar una pelicula en especifico 
        /// </summary>
        /// <param name="id_pelicula">ID de la pelicula a actualizar</param>
        /// <param name="pelicula_dto">Objeto con los nuevos datos a actualizar</param>
        /// <returns></returns>
        [HttpPatch("{id_pelicula:int}", Name = "ActualizarPelicula")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPelicula(int id_pelicula, [FromBody] PeliculaUpdateDto pelicula_dto)
        {
            //Comprobamos si el objeto enviado no es nulo y si el id es diferente 
            if (pelicula_dto == null || id_pelicula != pelicula_dto.Id)
            {
                return BadRequest(ModelState);
            }

            //Creamos un objeto de pelicula 
            var pelicula = _mapper.Map<Pelicula>(pelicula_dto);

            //Comprobamos si se pudo crear la pelicula con exito
            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar la pelicula {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return Content("Registro actualizado.");

        }


        //Metodo para borrar una pelicula 
        /// <summary>
        /// Borrar una pelicula 
        /// </summary>
        /// <param name="id_pelicula">ID de la pelicula a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id_pelicula:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarPelicula(int id_pelicula)
        {
            //Comprobamos si la pelicula existe 
            if (!_pelRepo.ExistePelicula(id_pelicula))
            {
                return NotFound();
            }

            //Obtenemos la pelicula encontrada 
            var pelicula = _pelRepo.GetPelicula(id_pelicula);

            //Comprobamos si se elimina la pelicula 
            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal al borrar la pelicula {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return Content("Registro borrado.");

        }

    }
}
