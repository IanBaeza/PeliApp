using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeliculasAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
            .ForMember(entidad => entidad.Foto, options => options.Ignore());

            CreateMap<CineCreacionDTO, Cine>()
                .ForMember(entidad => entidad.Ubicacíon, dto => dto.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Longitud, dto.Latitud))));

            CreateMap<Cine, CineDTO>()
                .ForMember(dto => dto.Latitud, entidad => entidad.MapFrom(e => e.Ubicacíon.Y))
                .ForMember(dto => dto.Longitud, entidad => entidad.MapFrom(e => e.Ubicacíon.X));

            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(entidad => entidad.Poster, opciones => opciones.Ignore())
                .ForMember(entidad => entidad.PeliculaGenero, opciones => opciones.MapFrom(MapearPeliculasGeneros))
                .ForMember(entidad => entidad.PeliculaCine, opciones => opciones.MapFrom(MapearPeliculasCines))
                .ForMember(entidad => entidad.PeliculaActor, opciones => opciones.MapFrom(MapearPeliculasActor));

            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(dto => dto.Generos, options => options.MapFrom(MapearPeliculasGenerosDTO))
                .ForMember(dto => dto.Cines, options => options.MapFrom(MapearPeliculasCinesDTO))
                .ForMember(dto => dto.Actores, options => options.MapFrom(MapearPeliculasActoresDTO));

            CreateMap<IdentityUser, UsuarioDTO>();
        }

        private List<PeliculaGenero> MapearPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO,
            Pelicula pelicula)
        {
            var listPeliculaGeneros = new List<PeliculaGenero>();

            if (peliculaCreacionDTO.GenerosIds == null) { return listPeliculaGeneros; }

            foreach (var generoIdDTO in peliculaCreacionDTO.GenerosIds)
            {
                listPeliculaGeneros.Add(new PeliculaGenero() { 
                    GeneroId = generoIdDTO 
                });
            }

            return listPeliculaGeneros;
        }

        private List<PeliculaCine> MapearPeliculasCines(PeliculaCreacionDTO peliculaCreacionDTO,
            Pelicula pelicula)
        {
            var listPeliculaCines = new List<PeliculaCine>();

            if (peliculaCreacionDTO.CinesIds == null) { return listPeliculaCines; }

            foreach (var cineIdDTO in peliculaCreacionDTO.CinesIds)
            {
                listPeliculaCines.Add(new PeliculaCine() { 
                    CineId = cineIdDTO 
                });
            }

            return listPeliculaCines;
        }

        private List<PeliculaActor> MapearPeliculasActor(PeliculaCreacionDTO peliculaCreacionDTO,
            Pelicula pelicula)
        {
            var listPeliculaActores = new List<PeliculaActor>();

            if (peliculaCreacionDTO.Actores == null) { return listPeliculaActores; }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                listPeliculaActores.Add(new PeliculaActor() { 
                    ActorId = actor.Id, 
                    Personaje = actor.Personaje 
                });
            }

            return listPeliculaActores;
        }

        private List<GeneroDTO> MapearPeliculasGenerosDTO(Pelicula peliculaEntity, PeliculaDTO peliculaDTO)
        {
            var listGenerosDTOs = new List<GeneroDTO>();

            if (peliculaEntity.PeliculaGenero != null)
            {
                foreach (var peliculaGenero in peliculaEntity.PeliculaGenero)
                {
                    listGenerosDTOs.Add(new GeneroDTO()
                    {
                        Id = peliculaGenero.GeneroId,
                        Nombre = peliculaGenero.Genero.Nombre
                    });
                }
            }

            return listGenerosDTOs;
        }

        private List<CineDTO> MapearPeliculasCinesDTO(Pelicula peliculaEntity, PeliculaDTO peliculaDTO)
        {
            var listCinesDTOs = new List<CineDTO>();

            if (peliculaEntity.PeliculaCine != null)
            {
                foreach (var peliculaCine in peliculaEntity.PeliculaCine)
                {
                    listCinesDTOs.Add(new CineDTO()
                    {
                        Id = peliculaCine.CineId,
                        Nombre = peliculaCine.Cine.Nombre,
                        Latitud = peliculaCine.Cine.Ubicacíon.Y,
                        Longitud = peliculaCine.Cine.Ubicacíon.X
                    });
                }
            }

            return listCinesDTOs;
        }

        private List<PeliculaActorDTO> MapearPeliculasActoresDTO(Pelicula peliculaEntity, PeliculaDTO peliculaDTO)
        {
            var listActoresDTOs = new List<PeliculaActorDTO>();

            if (peliculaEntity.PeliculaActor != null)
            {
                foreach (var peliculaActor in peliculaEntity.PeliculaActor)
                {
                    listActoresDTOs.Add(new PeliculaActorDTO()
                    {
                        Id = peliculaActor.ActorId,
                        Nombre = peliculaActor.Actor.Nombre,
                        Foto = peliculaActor.Actor.Foto,
                        Orden = peliculaActor.Orden,
                        Personaje = peliculaActor.Personaje
                    }); 
                }
            }

            return listActoresDTOs;
        }

        
    }
}

//Metodo .ForMember() se utiliza para mappear
//un atributo especifico de mi Base de Datos
//como Actor.foto o Cine.Ubicacion