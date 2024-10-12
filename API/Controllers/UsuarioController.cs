using Data;
using Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entidades;
using System.Net;
using System.Security.Cryptography;

namespace API.Controllers
{
    public class UsuarioController : BaseApiController
    {
        private readonly UserManager<UsuarioAplicacion> _userManager;//AplicationDbContext _db;  wpineda implementar identity
        private readonly ITokenServicio _tokenServicio;
        private ApiResponse _response;
        private readonly RoleManager<RolAplicacion> _rolManager;

        public UsuarioController(//AplicationDbContext db, wpineda implementar identity
                                 UserManager<UsuarioAplicacion> userManager, 
                                 ITokenServicio tokenServicio
                                 , ApiResponse response,
                                 RoleManager<RolAplicacion> rolManager)
        {
            //_db = db;
            _userManager = userManager;
            _tokenServicio = tokenServicio;
            _response = response;
            _rolManager = rolManager;
        }

        /*
[Authorize]
[HttpGet]
public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
{
   var usuarios = await _db.Usuarios.ToListAsync();
   return Ok(usuarios);
}

[Authorize]
[HttpGet("{id}")]
public async Task<ActionResult<Usuario>> GetUsuario(int id)
{
   var usuarios = await _db.Usuarios.FindAsync(id);
   return Ok(usuarios);
}  wpineda implementar identity */

        //[Authorize(Roles = "admin")]
        [Authorize(Policy = "adminrol")] //Agregar politica
        [HttpPost("registro")]
        public async Task<ActionResult<UsuarioDto>> Registro(RegistroDto registroDto)
        {
            if (await UsuarioExiste(registroDto.Username))
            {
                return BadRequest("Usuario ya esta Registrado.");
            }
            //using var hmac = new HMACSHA512(); wpineda implementar identity

            /*var usuario = new Usuario
             {
                 Username = registroDto.Username.ToLower(),
                 //PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registroDto.Password)),  wpineda implementar identity
                 //PasswordSalt = hmac.Key
             };
             /* _db.Usuarios.Add(usuario);
              await _db.SaveChangesAsync();  wpineda implementar identity  */

            var usuario = new UsuarioAplicacion
            {
                UserName = registroDto.Username.ToLower(),
                Email = registroDto.Email.ToLower(),
                Apellidos = registroDto.Apellidos.ToLower(),
                Nombres = registroDto.Nombres.ToLower()
            };

            var resultado = await _userManager.CreateAsync(usuario, registroDto.Password);

            if (!resultado.Succeeded)
            {
                return BadRequest(resultado.Errors);
            }

            var rolResultado = await _userManager.AddToRoleAsync(usuario, registroDto.Rol);

            if (!rolResultado.Succeeded)
            {
                return BadRequest("Error al agregar el Rol al Usuario.");
            }


            return new UsuarioDto
            {
                Username = registroDto.Username.ToLower(),
                Token = await _tokenServicio.crearToken(usuario)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login(LoginDto loginDto)
        {
            //var usuario = await _db.Usuarios.SingleOrDefaultAsync(x => x.Username == loginDto.Username);
            var usuario = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (usuario == null)
            {
                return Unauthorized("Usuario no Valido");
            }
            //  using var hmac = new HMACSHA512(usuario.PasswordSalt);   wpineda implementar identity  */
            var resultado = await _userManager.CheckPasswordAsync(usuario, loginDto.Password);

            if (!resultado)
            {
                return Unauthorized("passworld no valido");
            }

            /* var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != usuario.PasswordHash[i])
                {
                    return Unauthorized("Passworld no Valido");
                }
            }  wpineda implementar identity */

            return new UsuarioDto
            {
                Username = usuario.UserName,//usuario.Username.ToLower(), wpineda implementar identity
                Token = await _tokenServicio.crearToken(usuario)
            };
        }
        [Authorize(Policy = "adminrol")] //Agregar politica
        [HttpGet("ListadoRoles")]
        public IActionResult GetRoles()
        {
            var roles = _rolManager.Roles.Select(r => new { NombreRol = r.Name }).ToList();
            _response.Resultado = roles;
            _response.IsExitoso = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [AllowAnonymous]
        private async Task<bool> UsuarioExiste(string username)
        {
            //return await _db.Usuarios.AnyAsync(x => x.Username.Equals(username.ToLower()));  wpineda implementar identity
            return await _userManager.Users.AnyAsync(x => x.UserName.Equals(username.ToLower()));
        }
    }
}