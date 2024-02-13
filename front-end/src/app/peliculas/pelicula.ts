import { actorPeliculaDTO } from "../actores/actor";
import { cineDTO } from "../cines/cine";
import { generoDTO } from "../generos/genero";

export interface PeliculaCreacionDTO{
    titulo: string;
    resumen: string;
    enCines: boolean;
    trailer: string;
    fechaEstreno: Date;
    poster: File;
    generosIds: number[];
    cinesIds: number[];
    actores: actorPeliculaDTO[];
}

export interface PeliculaDTO{
    id: number;
    titulo: string;
    resumen: string;
    enCines: boolean;
    trailer: string;
    fechaEstreno: Date;
    poster: string;
    generos : generoDTO[];
    cines: cineDTO[];
    actores: actorPeliculaDTO[];
    votoUsuario : number;
    promedioVoto : number;
}

//Para poder seleccionar mis generos y mis cines en mi form de pelis
export interface PeliculaPostGetDTO{
    generos: generoDTO[];
    cines: cineDTO[];
}

export interface LandingPageDTO{
    pelisEnCinesDTO: PeliculaDTO[];
    pelisProximosEstrenosDTO: PeliculaDTO[];
}

export interface PeliculaPutGetDTO{
    pelicula : PeliculaDTO;
    generosSeleccionados: generoDTO[];
    generosNoSeleccionados: generoDTO[];
    cinesSeleccionados: cineDTO[];
    cinesNoSeleccionados: cineDTO[];
    actores: actorPeliculaDTO[];
}