import { Component } from '@angular/core';
import { PeliculasService } from '../peliculas/peliculas.service';
import { PeliculaDTO } from '../peliculas/pelicula';

@Component({
  selector: 'app-landing-page',
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.css']
})
export class LandingPageComponent {

  constructor(private peliculasService: PeliculasService){}

  peliculasEnCines: PeliculaDTO[];
  peliculasEstrenos: PeliculaDTO[];

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(){
    this.peliculasService.obtenerLandingPage()
      .subscribe(landigPage => {
        this.peliculasEnCines = landigPage.pelisEnCinesDTO;
        this.peliculasEstrenos = landigPage.pelisProximosEstrenosDTO;
      });
  }

  borrado(){
    this.cargarDatos();
  }
 
}
