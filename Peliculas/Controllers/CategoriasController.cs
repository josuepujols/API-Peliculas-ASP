using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models;
using Peliculas.Models.DTOS;
using Peliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Peliculas.Controllers
{
    [Route("api/Categorias")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculasCategorias")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaRepository _ctRepo;
        private readonly IMapper _mapper;
        //Contructor
        public CategoriasController(ICategoriaRepository ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        //Metodo para obtener todas las categorias
        /// <summary>
        /// Obtener todas las categorias
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<CategoriaDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategorias()
        {
            var lista_categorias = _ctRepo.GetCategorias();

            var lista_dto = new List<CategoriaDto>();
            foreach (var lista in lista_categorias)
            {
                lista_dto.Add(_mapper.Map<CategoriaDto>(lista));
            }

            return Ok(lista_dto);
        }


        //Metodo para traer una categoria por el id
        /// <summary>
        /// Obtener una categoria en especifico 
        /// </summary>
        /// <param name="id_categoria">Este es le id que se recibe para traer la categoria</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id_categoria:int}", Name = "GetCategoria")]
        [ProducesResponseType(200, Type = typeof(CategoriaDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetCategoria(int id_categoria)
        {
            var item_categoria = _ctRepo.GetCategoria(id_categoria);

            //Comprobamos si la categoria existe 
            if (item_categoria == null)
            {
                return NotFound();
            }
            else
            {
                var item_categoria_dto = _mapper.Map<CategoriaDto>(item_categoria);
                return Ok(item_categoria_dto);
            }

        }

        //Metodo para crear una categoria 
        /// <summary>
        ///  Crear una nueva categoria 
        /// </summary>
        /// <param name="categoria_dto">JSON con todos los campos para crear una nueva categoria</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearCategoria([FromBody] CategoriaDto categoria_dto) 
        {
            //Comprobamos si el objeto esta vacio 
            if (categoria_dto == null)
            {
                return BadRequest(ModelState);
            }

            //Comprobamos si la categoria existe 
            if (_ctRepo.ExisteCategoria(categoria_dto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe.");
                return StatusCode(404, ModelState);
            }

            var new_category = _mapper.Map<Categoria>(categoria_dto);

            //Comprobamos si se pudo crear la categoria con exito 
            if (!_ctRepo.CrearCategoria(new_category))
            {
                ModelState.AddModelError("", $"Algo salio mal al insertar la categoria {new_category.Nombre}");
                return StatusCode(500, ModelState);
            }

            //Si todo sale bien insertamos los datos 
            return CreatedAtRoute("GetCategoria", new { id_categoria = new_category.Id }, new_category);

        }

        //Metodo para actualizar una categoria 
        /// <summary>
        /// Actualizar uan categoria en especifico 
        /// </summary>
        /// <param name="id_categoria">ID de la categoria a actualizar</param>
        /// <param name="categoria_dto">Objeto con los nuevos datos a actualizar</param>
        /// <returns></returns>
        [HttpPatch("{id_categoria:int}", Name = "ActualizarCategoria")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarCategoria(int id_categoria, [FromBody]CategoriaDto categoria_dto)
        {
            //Comprobamos si el objeto enviado no es nulo y si el id es diferente 
            if (categoria_dto == null || id_categoria != categoria_dto.Id)
            {
                return BadRequest(ModelState);
            }

            //Creamos un objeto de categoria 
            var categoria = _mapper.Map<Categoria>(categoria_dto);

            //Comprobamos si se pudo crear la categoria con exito
            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar la categoria {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return Content("Registro actualizado.");

        }     


        //Metodo para borrar una categoria 
        /// <summary>
        /// Borrar una categoria existente 
        /// </summary>
        /// <param name="id_categoria">ID de la categoria a borrar</param>
        /// <returns></returns>
        [HttpDelete("{id_categoria:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int id_categoria)
        {
            //Comprobamos si la categoria existe 
            if (!_ctRepo.ExisteCategoria(id_categoria))
            {
                return NotFound();
            }

            //Obtenemos la categoria encontrada 
            var categoria = _ctRepo.GetCategoria(id_categoria);

            //Comprobamos si se elimina la categoria 
            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal al borrar la categoria {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return Content("Registro borrado.");

        }

    }
}
