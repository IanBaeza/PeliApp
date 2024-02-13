using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PeliculasAPI.DTOs;
using PeliculasAPI.Utilidades;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext contextDB;
        private readonly IMapper mapper;

        public CuentasController(
            UserManager<IdentityUser> userManager,      //Servicio para crear usuarios
            SignInManager<IdentityUser> signInManager,  //Sericio para logear usuarios
            IConfiguration configuration,
            ApplicationDbContext contextDB,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.contextDB = contextDB;
            this.mapper = mapper;
        }

        [HttpGet("ListadoUsuarios")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult<List<UsuarioDTO>>> ListadoUsuarios([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryableUsuario = contextDB.Users.AsQueryable();

            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryableUsuario);

            var usuariosDB = await queryableUsuario.OrderBy(usuario => usuario.Email)
                .Paginar(paginacionDTO)
                .ToListAsync();

            return mapper.Map<List<UsuarioDTO>>(usuariosDB);

        }

        [HttpPost("HacerAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> HacerAdmin([FromBody]string usuarioID)
        {
            var usuarioDB = await userManager
                .FindByIdAsync(usuarioID);

            await userManager
                .AddClaimAsync(usuarioDB, new Claim("role", "admin"));

            return NoContent();
        }

        [HttpPost("RemoverAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
        public async Task<ActionResult> RemoverAdmin([FromBody] string usuarioID)
        {
            var usuarioDB = await userManager
                .FindByIdAsync(usuarioID);

            await userManager
                .RemoveClaimAsync(usuarioDB, new Claim("role", "admin"));

            return NoContent();
        }

        [HttpPost("crear")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Crear([FromBody] CredencialesUsuarioDTO credencialesDTO)
        {
            //Crear usuario
            var usuarioDB = new IdentityUser { UserName = credencialesDTO.Email, 
                                             Email = credencialesDTO.Email};

            var resultado = await userManager.CreateAsync(usuarioDB, credencialesDTO.Password);

            //Crear token
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesDTO);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login([FromBody] CredencialesUsuarioDTO credencialesDTO)
        {
            //Validar usuario 
            var resultado = await signInManager.PasswordSignInAsync(
                                                    credencialesDTO.Email, 
                                                    credencialesDTO.Password, 
                                                    isPersistent: false, 
                                                    lockoutOnFailure: false);

            //Crear token
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesDTO);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }

        private async Task<ActionResult<RespuestaAutenticacionDTO>> ConstruirToken(CredencialesUsuarioDTO credencialesDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesDTO.Email)
            };

            var expiracion = DateTime.UtcNow.AddYears(1);

            var usuario = await userManager.FindByEmailAsync(credencialesDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creeds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            
            var jwtToken = new JwtSecurityToken(
                                issuer: null, 
                                audience: null, 
                                claims: claims, 
                                expires: expiracion, 
                                signingCredentials: creeds);

            return new RespuestaAutenticacionDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Expiracion = expiracion
            };
        }

    }
}
