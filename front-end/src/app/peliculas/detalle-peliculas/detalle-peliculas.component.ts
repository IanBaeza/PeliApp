import { Component, OnInit } from '@angular/core';
import { PeliculasService } from '../peliculas.service';
import { ActivatedRoute } from '@angular/router';
import { PeliculaDTO } from '../pelicula';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { CoordenadaConMensaje } from 'src/app/utilidades/mapa/coordenada';
import { RaitingService } from 'src/app/reiting/raiting.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-detalle-peliculas',
  templateUrl: './detalle-peliculas.component.html',
  styleUrls: ['./detalle-peliculas.component.css']
})
export class DetallePeliculasComponent implements OnInit {

  constructor(
    private peliculaService: PeliculasService,
    private raitingService: RaitingService,
    private activeRoute: ActivatedRoute,
    private sanitizer: DomSanitizer) { }

    pelicula: PeliculaDTO;
    fechaEstreno: Date;
    trailerURL: SafeResourceUrl;
    coordenadas: CoordenadaConMensaje[] = [];

  ngOnInit(): void {
    this.activeRoute.params.subscribe(params => {
      this.peliculaService.obtenerPorId(params.id).subscribe(pelicula =>{
        this.pelicula = pelicula;
        this.fechaEstreno = new Date(this.pelicula.fechaEstreno);
        this.trailerURL = this.generarURLYoutubeEmbed(this.pelicula.trailer);
        this.coordenadas = pelicula.cines.map(cine => {
          return {longitud: cine.longitud, latitud: cine.latitud, mensaje:cine.nombre}
        });
      })
    })
  }

  generarURLYoutubeEmbed(url: any): SafeResourceUrl{
    if(!url){
      return '';
    }

    var video_id = url.split('v=')[1];
    var posicionAmpersand = video_id.indexOf('&');
    if(posicionAmpersand !== -1){
      video_id = video_id.subscribe(0, posicionAmpersand);
    }

    return this.sanitizer.
      bypassSecurityTrustResourceUrl(`https://www.youtube.com/embed/${video_id}`);
  }

  puntuar(puntuacion: number){
    this.raitingService.puntuar(this.pelicula.id, puntuacion)
      .subscribe(() =>{
        Swal.fire("Exitoso", "Su voto ha sido recibido", 'success');
      });
  }

}
