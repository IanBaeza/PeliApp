using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]

    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly UserManager<IdentityUser> userManager;
        private readonly string contenedor = "peliculas";

        public PeliculasController(
            ApplicationDbContext context, 
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.userManager = userManager;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var peliculaDB = await context.Peliculas
                .Include(pelicula => pelicula.PeliculaGenero)
                    .ThenInclude(peliGenero => peliGenero.Genero)
                .Include(pelicula => pelicula.PeliculaCine)
                    .ThenInclude(peliCine => peliCine.Cine)
                .Include(pelicula => pelicula.PeliculaActor)
                    .ThenInclude(peliActor => peliActor.Actor)
                .FirstOrDefaultAsync(pelicula => pelicula.Id == id);

            if(peliculaDB == null) { return NotFound(); }

            var promedioVoto = 0.0;
            var usuarioVoto = 0;

            if (await context.Ratings.AnyAsync(raiting => raiting.PeliculId == id))
            {
                promedioVoto = await context.Ratings
                    .Where(r => r.PeliculId == id)
                    .AverageAsync(r => r.Puntuacion);

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var email = HttpContext.User.Claims
                        .FirstOrDefault(c => c.Type == "email")
                            .Value;

                    var usuarioIdentity = await userManager
                        .FindByEmailAsync(email);

                    var usuarioId = usuarioIdentity.Id;

                    var raitingDB = await context.Ratings
                        .FirstOrDefaultAsync(r => r.UsuarioId == usuarioId && 
                                                  r.PeliculId == id);

                    if(raitingDB != null)
                    {
                        usuarioVoto = raitingDB.Puntuacion;
                    }
                }  
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(peliculaDB);

            peliculaDTO.votoUsuario = usuarioVoto;
            peliculaDTO.PromedioVoto = promedioVoto;
            peliculaDTO.Actores = peliculaDTO.Actores
                .OrderBy(peliActorDTO => peliActorDTO.Orden)
                .ToList();

            return peliculaDTO;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<LandingPageDTO>> obtenerLandingPage()
        {
            var landingPageDTO = new LandingPageDTO();
            var top = 6;
            var fechaHoy = DateTime.Today;

            var pelisEnCinesEntity = await context.Peliculas.Where(peliEntity => peliEntity.EnCines)
                                                            .OrderBy(peliEntity => peliEntity.FechaEstreno)
                                                            .Take(top)
                                                            .ToListAsync();

            var pelisProximosEstrenosEntity = await context.Peliculas.Where(peliEntity => peliEntity.FechaEstreno > fechaHoy)
                                                                     .OrderBy(peliEntity => peliEntity.FechaEstreno)
                                                                     .Take(top)
                                                                     .ToListAsync();

            landingPageDTO.pelisEnCinesDTO = mapper.Map<List<PeliculaDTO>>(pelisEnCinesEntity);
            landingPageDTO.pelisProximosEstrenosDTO = mapper.Map<List<PeliculaDTO>>(pelisProximosEstrenosEntity);

            return landingPageDTO;
        }

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<PeliculasPutGetDTO>> PutGet(int id)
        {
            var getPeliculaDTO = await Get(id);

            if (getPeliculaDTO.Result is NotFoundResult) { return NotFound(); }

            var peliculaDTO = getPeliculaDTO.Value;

            var generosSeleccionadosIds = peliculaDTO.Generos.Select(generos => generos.Id)
                                                          .ToList();
            var generosNoSeleccionados = await context.Generos.Where(generos => !generosSeleccionadosIds.Contains(generos.Id))
                                                              .ToListAsync();

            var cinesSelecionadosIds = peliculaDTO.Cines.Select(cines => cines.Id)
                                                     .ToList();
            var cinesNoSeleccionados = await context.Cines.Where(cines => !cinesSelecionadosIds.Contains(cines.Id))
                                                          .ToListAsync();

            var generosNoSeleccionadosDTO = mapper.Map<List<GeneroDTO>>(generosNoSeleccionados);
            var cinesNoSeleccionadosDTO = mapper.Map<List<CineDTO>>(cinesNoSeleccionados);
            
            var peliculaPutGetDTO = new PeliculasPutGetDTO();

            peliculaPutGetDTO.Pelicula = peliculaDTO;
            peliculaPutGetDTO.GenerosSeleccionados = peliculaDTO.Generos;
            peliculaPutGetDTO.GenerosNoSeleccionados = generosNoSeleccionadosDTO;
            peliculaPutGetDTO.CinesSeleccionados = peliculaDTO.Cines;
            peliculaPutGetDTO.CinesNoSeleccionados = cinesNoSeleccionadosDTO;
            peliculaPutGetDTO.Actores = peliculaDTO.Actores;

            return peliculaPutGetDTO;
        }

        [HttpGet("PostGet")]
        public async Task<ActionResult<PeliculasPostGetDTO>> PostGet()
        {
            var generosEntitys = await context.Generos.ToListAsync();
            var cinesEntitys = await context.Cines.ToListAsync();

            var generosDTO = mapper.Map<List<GeneroDTO>>(generosEntitys);
            var cinesDTO = mapper.Map<List<CineDTO>>(cinesEntitys);

            return new PeliculasPostGetDTO() { Generos = generosDTO, Cines = cinesDTO };
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculasFiltrarDTO peliculasFiltrarDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(peliculasFiltrarDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable
                                     .Where(peli => peli.Titulo.Contains(peliculasFiltrarDTO.Titulo));

            }

            if (peliculasFiltrarDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable
                                     .Where(peli => peli.EnCines);
            }

            if (peliculasFiltrarDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable
                                     .Where(peli => peli.FechaEstreno > hoy);
            }

            if(peliculasFiltrarDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                                     .Where(peli => peli.PeliculaGenero.Select(f => f.GeneroId)
                                                    .Contains(peliculasFiltrarDTO.GeneroId));      
            }

            await HttpContext.InsertarParametrosPaginacionEnCabecera(peliculasQueryable);

            var peliculasEntitys = await peliculasQueryable
                                  .Paginar(peliculasFiltrarDTO.PaginacionDTO)
                                  .ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculasEntitys);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaEntity = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Poster != null)
            {
                peliculaEntity.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster);
            }

            EscribirOrdenActores(peliculaEntity);

            context.Add(peliculaEntity);
            await context.SaveChangesAsync();
            return peliculaEntity.Id;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaEntity = await context.Peliculas
                                            .Include(peliculaEntity => peliculaEntity.PeliculaGenero)
                                            .Include(peliculaEntity => peliculaEntity.PeliculaCine)
                                            .Include(peliculaEntity => peliculaEntity.PeliculaActor)
                                            .FirstOrDefaultAsync(peliculaEntity => peliculaEntity.Id == id);

            if (peliculaEntity == null) { return NotFound(); }

            peliculaEntity = mapper.Map(peliculaCreacionDTO, peliculaEntity);

            if (peliculaCreacionDTO.Poster != null)
            {
                peliculaEntity.Poster = await almacenadorArchivos.EditarArchivo(contenedor, peliculaCreacionDTO.Poster, peliculaEntity.Poster);
            }

            EscribirOrdenActores(peliculaEntity);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(peli => peli.Id == id);

            if(pelicula == null) { return NotFound(); }

            context.Remove(pelicula);
            await context.SaveChangesAsync();

            await almacenadorArchivos.BorrarArchivo(pelicula.Poster, contenedor);

            return NoContent();
        }

        private void EscribirOrdenActores(Pelicula peliculaEntity)
        {
            if(peliculaEntity.PeliculaActor != null)
            {
                for (int i = 0; i < peliculaEntity.PeliculaActor.Count; i++)
                {
                    peliculaEntity.PeliculaActor[i].Orden = i;
                }
            }
        }
    }
}
