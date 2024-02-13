import { Component, OnInit } from '@angular/core';
import { PeliculaCreacionDTO } from '../pelicula';
import { PeliculasService } from '../peliculas.service';
import { MultipleSelectorModel } from 'src/app/utilidades/selector-multiple/MultipleSelectorModel';
import { parsearErroresAPI } from 'src/app/utilidades/utilidades';
import { Router } from '@angular/router';

@Component({
  selector: 'app-crear-pelicula',
  templateUrl: './crear-pelicula.component.html',
  styleUrls: ['./crear-pelicula.component.css']
})
export class CrearPeliculaComponent implements OnInit {
  
  errores: string[] = [];
  
  constructor(
    private peliculasService: PeliculasService,
    private router: Router
    ){}

  generosNoSelecionados: MultipleSelectorModel[];
  cinesNoSelecionados: MultipleSelectorModel[];

  ngOnInit(): void {
    this.peliculasService.postGet()
      .subscribe( resultado => {
          this.generosNoSelecionados = resultado.generos.map( genero => 
            {return <MultipleSelectorModel>{llave: genero.id, valor: genero.nombre}}
          );

          this.cinesNoSelecionados = resultado.cines.map (cine => 
            {return <MultipleSelectorModel>{llave: cine.id, valor: cine.nombre}}
          );
        }, error => console.error(error)
      );
  }

  guardarCambios(pelicula: PeliculaCreacionDTO){
    this.peliculasService.crear(pelicula)
      .subscribe((id: number) => this.router.navigate(['/pelicula/'+ id]), 
      error => this.errores = parsearErroresAPI(error));
  }

}
