using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Peliculas.Models;
using Peliculas.Models.DTOS;
using Peliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Peliculas.Controllers
{
    [Route("api/Usuarios")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiUsuariosPeliculas")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        //Contructor
        public UsuariosController(IUsuarioRepository userRepo, IMapper mapper, IConfiguration config)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _config = config;

        }

        //Metodo para obtener todos los usuarios 
        /// <summary>
        /// Obtener todos los usuarios 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<UsuarioDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetUsuarios()
        {
            var lista_usuarios = _userRepo.GetUsuarios();

            var lista_dto = new List<UsuarioDto>();
            foreach (var lista in lista_usuarios)
            {
                lista_dto.Add(_mapper.Map<UsuarioDto>(lista));
            }

            return Ok(lista_dto);
        }


        //Metodo para traer una usuario por id
        /// <summary>
        /// Obtener un usuario por el ID
        /// </summary>
        /// <param name="id_usuario">ID del usuario a buscar</param>
        /// <returns></returns>
        [HttpGet("{id_usuario:int}", Name = "GetUsuario")]
        [ProducesResponseType(200, Type = typeof(UsuarioDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetUsuario(int id_usuario)
        {
            var item_usuario = _userRepo.GetUsuario(id_usuario);

            //Comprobamos si la categoria existe 
            if (item_usuario == null)
            {
                return NotFound();
            }
            else
            {
                var item_usuario_dto = _mapper.Map<UsuarioDto>(item_usuario);
                return Ok(item_usuario_dto);
            }

        }

        //Metodo para registrar un usuario 
        /// <summary>
        /// Registrar un nuevo usuario 
        /// </summary>
        /// <param name="usuarioAuthDto">Objeto con los campos del usuario a agregar</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(201, Type = typeof(UsuarioAuthDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Registro(UsuarioAuthDto usuarioAuthDto)
        {
            usuarioAuthDto.Usuario = usuarioAuthDto.Usuario.ToLower();

            if (_userRepo.ExisteUsuario(usuarioAuthDto.Usuario))
            {
                return BadRequest("El usuario ya existe.");
            }

            var usuarioACrear = new Usuario
            {
                UsuarioA = usuarioAuthDto.Usuario
            };

            var usuarioCreado = _userRepo.Registro(usuarioACrear, usuarioAuthDto.Password);
            return Ok(usuarioCreado);
        }

        //Metodo para loggearnos
        /// <summary>
        /// Hacer login y obtener un token 
        /// </summary>
        /// <param name="usuarioAuthLoginDto">Objeto con el usuario y la contraseña</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(200, Type = typeof(UsuarioAuthLoginDto))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login(UsuarioAuthLoginDto usuarioAuthLoginDto)
        {

            var usuarioDesdeRepo = _userRepo.Login(usuarioAuthLoginDto.Usuario, usuarioAuthLoginDto.Password);

            if (usuarioDesdeRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDesdeRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioDesdeRepo.UsuarioA.ToString())
            };


            //Generacion del token 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credenciales
            };


            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { 
                token = tokenHandler.WriteToken(token)
            });

        }

    }
}
