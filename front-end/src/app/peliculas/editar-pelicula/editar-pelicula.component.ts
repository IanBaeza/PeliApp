import { Component, OnInit } from '@angular/core';
import { PeliculaCreacionDTO, PeliculaDTO } from '../pelicula';
import { PeliculasService } from '../peliculas.service';
import { ActivatedRoute, Router} from '@angular/router';
import { MultipleSelectorModel } from 'src/app/utilidades/selector-multiple/MultipleSelectorModel';
import { actorPeliculaDTO } from 'src/app/actores/actor';

@Component({
  selector: 'app-editar-pelicula',
  templateUrl: './editar-pelicula.component.html',
  styleUrls: ['./editar-pelicula.component.css']
})
export class EditarPeliculaComponent implements OnInit
{
  constructor(private peliculasService: PeliculasService,
    private activatedRoute: ActivatedRoute,
    private router: Router){}

  peliculaDTO: PeliculaDTO;
  generosSeleccionados: MultipleSelectorModel[];
  generosNoSeleccionados: MultipleSelectorModel[];
  cinesSeleccionados: MultipleSelectorModel[];
  cinesNoSeleccionados: MultipleSelectorModel[];
  actoresSeleccionados: actorPeliculaDTO[];

  ngOnInit(): void 
  {
    this.activatedRoute.params.subscribe(parametros =>{
      this.peliculasService.putGet(parametros.id).subscribe(peliculaPutGet =>{
    
        this.peliculaDTO = peliculaPutGet.pelicula;

        this.generosSeleccionados = peliculaPutGet.generosSeleccionados.map(genero =>{
          return <MultipleSelectorModel>{llave: genero.id, valor: genero.nombre}
        });
        this.generosNoSeleccionados = peliculaPutGet.generosNoSeleccionados.map(genero =>{
          return <MultipleSelectorModel>{llave: genero.id, valor: genero.nombre}
        });

        this.cinesSeleccionados = peliculaPutGet.cinesSeleccionados.map (cine =>{
          return <MultipleSelectorModel>{llave: cine.id, valor: cine.nombre}
        });

        this.cinesNoSeleccionados = peliculaPutGet.cinesNoSeleccionados.map (cine =>{
          return <MultipleSelectorModel>{llave: cine.id, valor: cine.nombre}
        });

        this.actoresSeleccionados = peliculaPutGet.actores;

      })
    })
  }

  guardarCambios(pelicula: PeliculaCreacionDTO)
  {
    this.peliculasService.editar(this.peliculaDTO.id, pelicula).subscribe(()=>
      this.router.navigate(['/pelicula/'+this.peliculaDTO.id])    
    );
  }



}
